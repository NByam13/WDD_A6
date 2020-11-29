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
                string[] splitData = data.Split(' ');
                string path = splitData[1];
            }
        }
    }
}
