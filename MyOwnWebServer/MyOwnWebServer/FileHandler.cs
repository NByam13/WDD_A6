﻿using System;
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
            //if the file doesn't exist
            if(!File.Exists(path))
            {
                //create a new file at the location
                var stream = File.Create(path);
                //close the file
                stream.Close();
            }
            //if it does exist
            else
            {
                //delete the file
                File.Delete(path);
                //create the file
                var stream = File.Create(path);
                //close the file
                stream.Close();
            }
        }

        static public bool IsValidPath(string path, out string code)
        {
            //if they ask for a log
            if(path.Contains(".log")) // The user is not allowed to request the log file from the server
            {
                //don't let them use the log
                code = HttpHandler.HTTPCodes.Forbidden;
                return false;
            }

            code = "";
            try // this will basically check to see if the path is valid, and is easier than creating a regex for the path.
            {
                if(path.EndsWith(".txt") || path.EndsWith(".html"))
                {
                    string tmp = File.ReadAllText(path);
                    if(tmp != "")
                    {
                        return true;
                    }
                    else { return false; }
                }
                else
                {
                    byte[] tmp = File.ReadAllBytes(path);
                    if(tmp != null)
                    {
                        return true;
                    }
                    else { return false; }
                }
                
            }
            catch(Exception e) // catch any exception that this this may throw, they're all related to path validity
            {
                Logger.Log(e.Message);
                return false;
            }
        }


        static public string GetTextResource(string path)
        {
            string resource;

            try
            {
                resource = File.ReadAllText(path);
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
