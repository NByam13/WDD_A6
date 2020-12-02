//*********************************************
// File			 : Logger.cs
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
using System.Diagnostics;

namespace MyOwnWebServer
{
    //******************************************
    // Name     : Logger
    // Purpose  : A class which is tasked with writing relevant details to the server log file. The file is created if it doesn't
    //          : exist, and there is a method that formats the log entry to adhere to project specifications.
    //******************************************
    static public class Logger
    {
        //declare variables
        private const string path = "./MyOwnWebServer.log";
        private static int startFlag = 1;

        /////////////////////////////////////////
        // Method       : Log
        // Description  : Sends the text to the specified log file, as well as connects the time to the message
        // Parameters   : string msg : The messages that will be sent to the log  
        // Returns      : N/A
        /////////////////////////////////////////
        static public void Log(string msg)
        {
            string timeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")).ToString(); // time stamp the log message using eastern standard time.
            string logMsg = timeStamp + " " + msg + "\n";

            if(startFlag == 1) // This is used to stop the Log from overwriting itself, so a new log file is only created on server start.
            {
                FileHandler.CreateFile(path);
                startFlag = 0;
            }
            try
            {
                File.AppendAllText(path, logMsg);
            }
            catch(Exception e) // if there is a write error then the error will be written to the Event log instead of the text log.
            {
                EventLog eventlog = new EventLog();
                if(!EventLog.SourceExists("WebServerEventSource"))
                {
                    EventLog.CreateEventSource("WebServerEventSource", "WebServerEvents");
                }
                eventlog.Source = "WebServerEventSource";
                eventlog.Log = "WebServerEvents";
                eventlog.WriteEntry(e.Message);
            }
        }

        /////////////////////////////////////////
        // Method       : FormatForLog
        // Description  : Formats the text that will be sent to the log
        // Parameters   : string[] msgs : The messages that will be sent to the log 
        //              : string status : tells the program why it was called
        //              : in order to put the proper information to the log
        // Returns      : string formattedMsg: The message that will be sent to the log
        /////////////////////////////////////////
        static public string FormatForLog(string[] msgs, string status)
        {
            string formattedMsg = "";
            if(status == "START") // This overloaded format method only handles the start command, because it has a list of cmd line arguments
            {                     // that must be recorded
                formattedMsg = "[SERVER STARTED]: ";
                foreach(string msg in msgs) // list commands in order they were given
                {
                    if(msg.StartsWith("-"))
                    {
                        formattedMsg += msg + "=";
                    }
                    else
                    {
                        formattedMsg += msg + " ";
                    }
                }
            }
            else // if a list is passed in but the status is not start, then the other format method will format a log message for the unknown
            {    // occurence.
                formattedMsg = FormatForLog("Unknown Operation", status);
            }

            return formattedMsg;
        }

        /////////////////////////////////////////
        // Method       : FormatForLog
        // Description  : Formats the text that will be sent to the log
        // Parameters   : string msg : The message that will be sent to the log 
        //              : string status : tells the program why it was called
        //              : in order to put the proper information to the log
        // Returns      : string formattedMsg: The message that will be sent to the log
        /////////////////////////////////////////
        static public string FormatForLog(string msg, string status)
        {
            string formattedMsg = "";
            if (status == "STOP") // if the server is given a stop request. This is unimplemented.
            {
                formattedMsg = "[SERVER STOPPED]: ";
                formattedMsg += msg;
            }
            else if (status == "RECEIVE") // If the reason for the log is that data was received
            {
                formattedMsg = "[RECEIVED]: ";
                try
                {
                    int index = msg.IndexOf("HTTP"); // we only want the verb and the resource to get
                    msg = msg.Substring(0, index - 1);
                }
                catch(Exception e) // if a weird response comes in where the verb and path can't be parsed, format an exception log.
                {
                    string tmp = FormatForLog("Bad Request Received: ", "EXCEPTION");
                    msg = tmp + msg;
                }
                formattedMsg += msg;
            }
            else if (status == "RESPONSE") // if the reason for the log is that a response has been formed
            {
                formattedMsg = "[RESPONSE]: ";
                if (msg.Contains("200") || msg.Contains("204")) // if the request was processed without error
                {
                    int start = msg.IndexOf('\n') + 1; // disregard first line, start at second line
                    string tmp = msg.Substring(start); // grab every line after the first in a substring
                    tmp = tmp.Replace('\r', ' '); // replace all occurences of \r
                    tmp = tmp.Replace('\n', ' '); // replace all occurences of \n
                    msg = tmp.Trim(); // trim whitespace from beginning and end.
                }
                else // if some kind of error code is given from the client's request
                {
                    try
                    {
                        int codeStart = msg.IndexOf(' ') + 1; // find index of status code beginning
                        int codeEnd = msg.IndexOf("\r\n"); // find status code end
                        msg = msg.Substring(codeStart, codeEnd - codeStart); // extract the status code
                    }
                    catch(Exception e) // this will trigger if the data sent it empty
                    {
                        string tmp = FormatForLog("The data sent from the client was empty. Status code: ", "EXCEPTION");
                        msg = tmp + HttpHandler.HTTPCodes.BadRequest;
                    }
                }
                formattedMsg += msg;
            }
            else if(status == "EXCEPTION") // if the reason for the log is to record an exception.
            {
                formattedMsg = "[EXCEPTION]: ";
                formattedMsg += msg;
            }
            else // if something strange happens, this log message is sent.
            {
                formattedMsg = "[UNKNOWN]: ";
                formattedMsg += status + " - ";
                formattedMsg += msg;
            }
            return formattedMsg;
        }
    }
}
