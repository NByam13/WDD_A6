using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Security;

namespace MyOwnWebServer
{
    static public class FileHandler
    {
        static public void CreateFile(string path) // only really used for the logger
        {
            if(!File.Exists(path))
            {
                var stream = File.Create(path);
                stream.Close();
            }
            else
            {
                File.Delete(path);
                var stream = File.Create(path);
                stream.Close();
            }
        }

        static public bool IsValidPath(string path)
        {
            if(path.Contains(".log")) // The user is not allowed to request the log file from the server
            {
                return false;
            }

            try // this will basically check to see if the path is valid, and is easier than creating a regex for the path.
            {
                Path.GetFullPath(path);
            }
            catch(Exception e) // catch any exception that this this may throw, they're all related to path validity
            {
                Logger.Log(e.Message);
                return false;
            }

            return true; // if the path doesn't throw any exceptions then it is a valid path.
        }


        static public string[] GetResource(string path)
        {
            string[] resource = new string[1024];

            try
            {
                resource = File.ReadAllLines(path);
            }
            catch(Exception e)
            {
                Logger.Log(e.Message);
                return null;
            }
            return resource;
        }
    }
}
