namespace librsvgdotnet
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.ExceptionServices;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// The SVGImage class allows a C# Bitmap to be rendered from a string containing SVG data
    /// </summary>
    public class SVGImage : IDisposable
    {
        private IntPtr _rsvgHandle = IntPtr.Zero;
        private static bool _initialized = false;
        
        /// <summary>
        /// Create a new SVGImage instance using a string containing SVG data
        /// </summary>
        /// <param name="data">The SVG data</param>
        public SVGImage(string data)
        {
            if (!_initialized)
            {
                NativeMethods.g_type_init();
                NativeMethods.rsvg_init();
                _initialized = true;
            }

            ReadFile(data);
        }

        /// <summary>
        /// Creates a C# Bitmap from the SVGImage
        /// </summary>
        /// <param name="w">The desired width</param>
        /// <param name="h">The desired height</param>
        /// <param name="stretch">If true, stretch the SVG image to fit the width and height exactly</param>
        /// <returns>A new Bitmap containing the SVG image</returns>
        public Bitmap Image(int w, int h, bool stretch)
        {
            NativeMethods.RsvgDimensionData dim = new NativeMethods.RsvgDimensionData();

            int dw = 0;
            int dh = 0;

            if (_rsvgHandle == IntPtr.Zero)
            {
                return null;
            }

            NativeMethods.rsvg_handle_get_dimensions(_rsvgHandle, ref dim);

            double scaleX = w / ((double)dim.width);
            double scaleY = h / ((double)dim.height);

            if (stretch)
            {
                dw = w;
                dh = h;
            }
            else
            {
                double fixedScale = (scaleX < scaleY ? scaleX : scaleY);
                double fixedWidth = dim.width * fixedScale;
                double fixedHeight = dim.height * fixedScale;
                scaleX = fixedScale;
                scaleY = fixedScale;
                dw = (int)fixedWidth;
                dh = (int)fixedHeight;
            }

            //// Initialize the gdk_pixbuf
            IntPtr pixbuf = NativeMethods.gdk_pixbuf_new(NativeMethods.ColorSpace.Rgb, true, 8, dw, dh);
            int stride = NativeMethods.gdk_pixbuf_get_rowstride(pixbuf);
            int width = dw;
            int height = dh;

            IntPtr pixels = NativeMethods.gdk_pixbuf_get_pixels(pixbuf);
            byte[] src = new byte[stride * height];
            Marshal.Copy(src, 0, pixels, src.Length);

            //// Create the cairo surface
            IntPtr surface = NativeMethods.cairo_image_surface_create_for_data(NativeMethods.gdk_pixbuf_get_pixels(pixbuf), 0, width, height, stride);
            IntPtr cairo = NativeMethods.cairo_create(surface);

            //// Set the scale and render the image
            NativeMethods.cairo_scale(cairo, scaleX, scaleY);
            NativeMethods.rsvg_handle_render_cairo(_rsvgHandle, cairo);

            //// Destroy the cairo surface
            NativeMethods.cairo_destroy(cairo);
            NativeMethods.cairo_surface_destroy(surface);

            //// Copy the gdk_pixbuf into a bitmap
            Bitmap bitmap = new Bitmap(width, height);
            byte[] temp = new byte[stride * height];
            Marshal.Copy(pixels, temp, 0, temp.Length);

            BitmapData bd = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            if (bd.Stride == stride)
            {
                Marshal.Copy(temp, 0, bd.Scan0, temp.Length);
            }
            else
            {
                for (int y = 0; y < height; ++y)
                {
                    Marshal.Copy(temp, y * stride, new IntPtr(bd.Scan0.ToInt64() + y * bd.Stride), width * 4);
                }
            }

            bitmap.UnlockBits(bd);

            //// Release the gdk_pixbuf
            NativeMethods.g_object_unref(pixbuf);

            return bitmap;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void ReadFile(string data)
        {
            IntPtr error = IntPtr.Zero;
            IntPtr handle = IntPtr.Zero;

            try
            {
                handle = NativeMethods.rsvg_handle_new_from_data(data, data.Length - 1, out error);
            }
            catch (Exception e)
            {
                throw e;
            }

            if (handle == IntPtr.Zero)
            {
                if (error != IntPtr.Zero)
                {
                    NativeMethods.GError errorstruct = (NativeMethods.GError)Marshal.PtrToStructure(error, typeof(NativeMethods.GError));
                    throw new Exception(errorstruct.message);
                }
                throw new Exception();
            }
            else
            {
                _rsvgHandle = handle;
            }
        }

        /// <summary>
        /// Free the memory associated with the SVGImage
        /// </summary>
        public void Dispose()
        {
            if (_rsvgHandle != IntPtr.Zero)
            {
                NativeMethods.rsvg_handle_free(_rsvgHandle);
            }
            _rsvgHandle = IntPtr.Zero;
        }


    }


}
