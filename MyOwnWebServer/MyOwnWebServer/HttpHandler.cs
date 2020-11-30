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
        public struct HTTPCodes
        {
            public const string OK = "200 OK";
            public const string NoContent = "204 No Content";
            public const string BadRequest = "400 Bad Request";
            public const string Forbidden = "403 Forbidden";
            public const string NotFound = "404 Not Found";
            public const string NotAllowed = "405 Method Not Allowed";
            public const string TeaPot = "418 I'm a Teapot";
            public const string TooMany = "429 Too Many Requests";
            public const string ServerErr = "500 Internal Server Error";
            public const string NotImplemented = "501 Not Implemented";
            public const string VersionErr = "505 HTTP Version Not Supported";

            public string currentCode;
        }
        

        public const string version = "HTTP/1.1";

        public static bool ValidateRequest(string data, out string resource, out string code)
        {
            if(data == null)
            {
                resource = null;
                code = HTTPCodes.BadRequest;
                return false;
            }
            if (data.StartsWith("GET")) // this means it is a GET request
            {
                if(data.Contains("HTTP/1.1\r\n"))
                {
                    string[] splitData = data.Split(' ');
                    string path = splitData[1];
                    resource = path;
                    code = HTTPCodes.OK;
                    return true;
                }
                else
                {
                    // not a valid header if it does not have this
                    resource = null;
                    code = HTTPCodes.VersionErr;
                    return false;
                }
            }
            else
            {
                // we don't support post
                resource = null;
                code = HTTPCodes.NotAllowed;
                return false;
            }
            
        }



        public static byte[] BuildResponse(string mime, string code, int length)
        {
            string time = DateTime.UtcNow.ToLongDateString();

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("HTTP/1.1 {0}\r\n", code);
            builder.AppendFormat("Content-Type: {0}\r\n", mime);
            builder.Append("Server: MyOwnWebServer\r\n");
            builder.AppendFormat("Date: {0}\r\n", time);
            builder.AppendFormat("Content-Length: {0}\r\n", length);
            builder.Append("\r\n");

            return Encoding.ASCII.GetBytes(builder.ToString());
        }


        public static byte[] Converter(string path)
        {
            byte[] bytes = new byte[1000096];
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
            //if html or text
            else if (path.EndsWith(".html") || path.EndsWith(".txt"))
            {
                string htmlString = FileHandler.GetTextResource(path);
                bytes = Encoding.ASCII.GetBytes(htmlString);
            }
            else
            {
                bytes = null;
            }
            //return the value in byte array
            return bytes;
        }
    }
}
