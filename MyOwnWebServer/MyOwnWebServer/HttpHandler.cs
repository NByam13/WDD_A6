using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOwnWebServer
{
    static public class HttpHandler
    {
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
    }
}
