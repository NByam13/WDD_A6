using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace MyOwnWebServer
{
    static public class HttpHandler
    {

        public struct HTTPCodes
        {
            //holds all httpcodes
            public const string OK = "200 OK";
            public const string NoContent = "204 No Content";
            public const string BadRequest = "400 Bad Request";
            public const string Forbidden = "403 Forbidden";
            public const string NotFound = "404 Not Found";
            public const string NotAllowed = "405 Method Not Allowed";
            public const string TeaPot = "418 I'm a Teapot";
            public const string ServerErr = "500 Internal Server Error";
            public const string NotImplemented = "501 Not Implemented";
            public const string VersionErr = "505 HTTP Version Not Supported";

            //returns code required by program
            public string currentCode;
        }
        
        //holds the version
        public const string version = "HTTP/1.1";

        public static bool ValidateRequest(string data, out string resource, out string code)
        {
            //if the data is empty
            if(data == "")
            {
                //go to the 400 error code
                resource = "returnHtml/400.html";
                //store error code in value
                code = HTTPCodes.BadRequest;
                //return false to say there was a bad request
                return false;
            }
            if (data.StartsWith("GET")) // this means it is a GET request
            {
                //if it contains HTTP/1.1
                if(data.Contains("HTTP/1.1\r\n"))
                {
                    // splits data based on the position of the path in the header.
                    int pathStart = data.IndexOf(' ') + 1;
                    int pathEnd = data.IndexOf("HTTP/1.1") - 1;
                    string path = data.Substring(pathStart, pathEnd - pathStart);

                    // This happens if coffee is requested from the server, we don't serve coffee, only tea... Sorry Sean.
                    if(path.ToLower().Contains("coffee"))
                    {
                        code = HTTPCodes.TeaPot;
                        resource = "returnHtml/418.html";
                        return false;
                    }

                    //if the path starts with /
                    if(path.StartsWith("/"))
                    {
                        //trim the path
                        path = path.Trim();
                        // delete the preceeding slash
                        path = path.Remove(0, 1);
                    }
                    //if the path starts with \\
                    else if(path.StartsWith("\\"))
                    {
                        //trim the path
                        path = path.Trim();
                        //replace it with a blank line...
                        path = path.Remove(0, 1);
                    }

                    //if the path contains favicon
                    if(path.Contains("favicon.ico"))
                    {
                        //tell the program that theres no content
                        code = HTTPCodes.NoContent;
                        //empty the resource
                        resource = null;
                        //return true
                        return true;
                    }

                    // Check to see if the uri for the resource is a valid uri
                    if(!Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
                    {
                        resource = "returnHtml/400.html";
                        code = HTTPCodes.BadRequest;
                        return false;
                    }
                    //make the modified path the resource
                    resource = path;
                    //return the OK error code to tell the logger everything is fine
                    code = HTTPCodes.OK;
                    //return true
                    return true;
                }
                else
                {
                    // not a valid header if it does not have this
                    resource = "returnHtml/505.html";
                    //get the version error (error 505)
                    code = HTTPCodes.VersionErr;
                    //return false to say there was an issue
                    return false;
                }
            }
            else
            {
                // we don't support post
                resource = "returnHtml/501.html";
                code = HTTPCodes.NotImplemented;
                return false;
            }
            
        }



        public static byte[] BuildResponse(string mime, string code, int length)
        {
            //tell the current date
            string time = DateTime.UtcNow.ToLongDateString() + " " + DateTime.UtcNow.ToLongTimeString();
            //Create the http header
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("HTTP/1.1 {0}\r\n", code);
            builder.AppendFormat("Content-Type: {0}\r\n", mime);
            builder.Append("Server: MyOwnWebServer\r\n");
            builder.AppendFormat("Date: {0}\r\n", time);
            builder.AppendFormat("Content-Length: {0}\r\n", length);
            builder.Append("\r\n");

            Logger.Log(Logger.FormatForLog(builder.ToString(), "RESPONSE"));
            //return the header
            return Encoding.ASCII.GetBytes(builder.ToString());
        }


        public static byte[] Converter(string path)
        {
            byte[] bytes = new byte[1000096];
            //if .gif
            if (path.EndsWith(".gif"))
            {
                bytes = File.ReadAllBytes(path);
            }
            //if .jpg .other image formats
            else if((path.EndsWith(".jpeg"))|| (path.EndsWith(".jpg"))|| (path.ToLower().EndsWith(".png")))
            {
                bytes = File.ReadAllBytes(path);
            }
            //if html or text
            else if (path.EndsWith(".html") || path.EndsWith(".txt"))
            {
                string htmlString = FileHandler.GetTextResource(path);
                bytes = Encoding.ASCII.GetBytes(htmlString);
            }
            // For when the browser sends a request for the tab icon
            else if(path.Contains("favicon.ico"))
            {
                string content = "";
                bytes = Encoding.ASCII.GetBytes(content);
            }
            //if its none of the following files send an empty byte array
            else
            {
                bytes = null;
            }
            //return the value in byte array
            return bytes;
        }
    }
}
