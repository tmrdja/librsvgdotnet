using librsvgdotnet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            String file = "test.svg";
            string text = File.ReadAllText(file, Encoding.GetEncoding("iso-8859-1"));
            /*text = Regex.Replace(text, "(<!--(.*?)-->)|(ng-[a-zA-Z0-9-]+=\"[^\"]*\")", "");
            byte[] bytes = Encoding.Default.GetBytes(text);
            text = Encoding.UTF8.GetString(bytes);*/
            //File.WriteAllText("loadPodloga2.svg", text);
            SVGImage image = new SVGImage(text + "\n");
            FileStream sw = File.Create("svg.pdf");
            image.SaveToPdf(sw);

            image.SaveToPdf("svg2.pdf");
            //System.Drawing.Bitmap i = image.Image(10.0);
            System.Drawing.Bitmap i = image.Image(3000, 3000, false);

            Bitmap target = new Bitmap(i.Size.Width, i.Size.Height);
            Graphics g = Graphics.FromImage(target);
            g.Clear(Color.White);
            //g.DrawRectangle(new Pen(new SolidBrush(Color.White)), 0, 0, target.Width, target.Height);
            g.DrawImage(i, 0, 0);
            target.Save("svg.png", ImageFormat.Png);
            i.Save("svg2.png", ImageFormat.Png);
            i.Save("svg.jpeg", ImageFormat.Jpeg);
            i.Save("svg.bmp", ImageFormat.Bmp);
        }
    }
}
