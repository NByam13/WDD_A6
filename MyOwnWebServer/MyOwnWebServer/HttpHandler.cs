using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOwnWebServer
{
    static public class HttpHandler
    {
        public struct HTTPCodes
        {
            const string OK = "200 OK";
            const string NoContent = "204 No Content";
            const string BadRequest = "400 Bad Request";
            const string Forbidden = "403 Forbidden";
            const string NotFound = "404 Not Found";
            const string NotAllowed = "405 Method Not Allowed";
            const string TeaPot = "418 I'm a Teapot";
            const string TooMany = "429 Too Many Requests";
            const string ServerErr = "500 Internal Server Error";
            const string NotImplemented = "501 Not Implemented";
            const string VersionErr = "505 HTTP Version Not Supported";

        }

        public static bool ValidateRequest(string data)
        {
            if(data.StartsWith("GET"))
            {
                // this means it is a GET request
                if(data.EndsWith("HTTP/1.1\r\n"))
                {
                    string[] splitData = data.Split(' ');
                    string path = splitData[1];
                    // Also need to check the last element is http/1.1 \r\n
                    if (FileHandler.IsValidPath(path))
                    {
                        // is a valid header
                        return true;
                    }
                    else
                    {
                        // is not a valid path
                        return false;
                    }
                }
                else
                {
                    // not a valid header if it does not have this
                    return false;
                }
            }
            else
            {
                // we don't support post
                return false;
            }
            
        }

        public static string BuildResponse()
        {

        }
    }
}
