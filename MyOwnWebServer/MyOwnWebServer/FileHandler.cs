//*********************************************
// File			 : FileHandler.cs
// Project		 : WDD - A6 - WebServer
// Programmer	 : Nick Byam, Nikola Ristic
// Last Change   : 2020-12-01
//*********************************************

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
    //******************************************
    // Name     : FileHandler
    // Purpose  : A class tasked with all program execution related to reading and verifying files that are to be sent back to the client
    //******************************************
    static public class FileHandler
    {
        /////////////////////////////////////////
        // Method       : CreateFile
        // Description  : Determines if the file is created, and if it is delete the file and 
        //              : create a new log, and if not, just create a new file
        // Parameters   : string path : The path given by the user
        // Returns      : N/A
        /////////////////////////////////////////
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

        /////////////////////////////////////////
        // Method       : IsValidPath
        // Description  : Determines if the path that is asked for is a valid path for the program
        // Parameters   : string path : The path given by the user
        // Returns      : string resource: returns the all the text found in the file
        //              : out string code: Used to set a 403 code if applicable, else it is an empty string that is handled
        //              :                : in the calling location.
        /////////////////////////////////////////
        static public bool IsValidPath(string path, out string code)
        {
            //if they ask for a log
            if(path.Contains(".log") || path.ToLower().Contains("returnhtml")) // The user is not allowed to request the log file from the server
            {                                                                  // The user can't explicitly request error code htmls either
                //don't let them use the log
                code = HttpHandler.HTTPCodes.Forbidden;
                return false;
            }

            // this satisfies the use of out that was added to handle a specific 403 Forbidden status condition
            code = ""; // at the calling location there is a condition block to check if a code was actually given.
            try // this will basically check to see if the path is valid, and is easier than creating a regex for the path.
            {
                //if the file is a txt or html
                if(path.EndsWith(".txt") || path.EndsWith(".html"))
                {
                    //read the file
                    string tmp = File.ReadAllText(path);
                    //if its not empty
                    if(tmp != "")
                    {
                        //return true
                        return true;
                    }
                    //if its empty return false
                    else { return false; }
                }
                //if its not one of them
                else
                {
                    //send all the data to the tmp
                    byte[] tmp = File.ReadAllBytes(path);
                    //if its not empty
                    if(tmp != null)
                    {
                        //tell the program its a valid path
                        return true;
                    }
                    //if its not valid, tell the program
                    else { return false; }
                }
                
            }
            catch(Exception e) // catch any exception that this this may throw, they're all related to path validity
            {
                //send the logger the exception that was caught
                Logger.Log(Logger.FormatForLog(e.Message, "EXCEPTION"));
                return false;
            }
        }

        /////////////////////////////////////////
        // Method       : GetTextResource
        // Description  : Gets the text from the file that on the path
        // Parameters   : string path : The path to the file
        // Returns      : string resource: returns the all the text found in the file
        /////////////////////////////////////////
        static public string GetTextResource(string path)
        {
            //declare variables
            string resource;

            try
            {
                //read the file and get the information
                resource = File.ReadAllText(path);
            }
            catch(Exception e)//if an error occurs
            {
                //send it to the logger
                Logger.Log(Logger.FormatForLog(e.Message, "EXCEPTION"));
                return null;
            }
            //return the string
            return resource;
        }
    }
}
