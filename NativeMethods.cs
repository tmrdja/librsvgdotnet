
using System;
using System.Runtime.InteropServices;


namespace librsvgdotnet
{
    class NativeMethods
    {

        const string libRSVG = "librsvg-2-2.dll";
        const string libCairo = "libcairo-2.dll";
        const string libgObject = "libgobject-2.0-0.dll";
        const string libGdkPixBuf = "libgdk_pixbuf-2.0-0.dll";


        internal enum ColorSpace { Rgb };

        [StructLayout(LayoutKind.Sequential)]
        internal struct RsvgDimensionData
        {
            public int width;
            public int height;
            public double em;
            public double ex;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct GError
        {
            int domain;
            int code;
            public string message;
        }


        [DllImport(libRSVG, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void rsvg_init();
        
        [DllImport(libRSVG, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr rsvg_handle_new_from_data([MarshalAs(UnmanagedType.LPStr)]string data, int data_len, out IntPtr error);
        
        [DllImport(libRSVG, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr rsvg_handle_new_from_file([MarshalAs(UnmanagedType.LPStr)]string file_name, out IntPtr error);
        
        [DllImport(libRSVG, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void rsvg_handle_free(IntPtr handle);
        
        [DllImport(libRSVG, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool rsvg_handle_render_cairo(IntPtr handle, IntPtr cairo);
        
        [DllImport(libRSVG, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void rsvg_handle_get_dimensions(IntPtr handle, ref RsvgDimensionData dimension_data);

        [DllImport(libCairo, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr cairo_create(IntPtr target);
        
        [DllImport(libCairo, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_destroy(IntPtr cairo);
        
        [DllImport(libCairo, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_surface_destroy(IntPtr cairo);
        
        [DllImport(libCairo, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_scale(IntPtr cairo, double w, double h);
        
        [DllImport(libCairo, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr cairo_image_surface_create_for_data(IntPtr data, int format, int width, int height, int stride);

        [DllImport(libgObject, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void g_type_init();
        
        [DllImport(libgObject, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void g_object_unref(IntPtr obj);

        [DllImport(libGdkPixBuf, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr gdk_pixbuf_get_pixels(IntPtr pixbuf);
        
        [DllImport(libGdkPixBuf, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int gdk_pixbuf_get_rowstride(IntPtr pixbuf);
        
        [DllImport(libGdkPixBuf, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr gdk_pixbuf_new(ColorSpace colorspace, bool has_alpha, int bits_per_sample, int width, int height);


    }


}
