using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace MyOwnWebServer
{
    static public class HttpHandler
    {
        public static bool ValidateRequest(string data)
        {
            if (data.StartsWith("GET"))
            {
                // this means it is a GET request
                string[] splitData = data.Split(' ');
                string path = splitData[1];
            }
        }
        public static byte[] Converter(string path)
        {
            byte[] bytes = new byte[4096];
            //if .gif
            if (path.EndsWith(".gif"))
            {
                Bitmap gifBM = new Bitmap(path);
                ImageConverter gifC = new ImageConverter();
                Image specifiedGIF = gifBM;
                bytes = (byte[])gifC.ConvertTo(specifiedGIF, typeof(byte[]));

            }
            //if .jpg .other image formats
            else if((path.EndsWith(".jpeg"))|| (path.EndsWith(".jpg"))|| (path.EndsWith(".png")))
            {
                Bitmap imageBM = new Bitmap(path);
                ImageConverter imageC = new ImageConverter();
                Image specifiedImage= imageBM;
                bytes = (byte[])imageC.ConvertTo(specifiedImage, typeof(byte[]));

            }
            //if html
            else if (path.EndsWith(".html"))
            {

            }
            //return the value in byte array
            return bytes;
        }
    }
}
