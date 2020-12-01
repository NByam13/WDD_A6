using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace MyOwnWebServer
{
    static public class Logger
    {
        private const string path = "./MyOwnWebServer.log";
        private static int startFlag = 1;

        static public void Log(string msg)
        {
            string timeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")).ToString();
            string logMsg = timeStamp + " " + msg + "\n";

            if(startFlag == 1)
            {
                FileHandler.CreateFile(path);
                startFlag = 0;
            }
            try
            {
                File.AppendAllText(path, logMsg);
            }
            catch(Exception e)
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

        static public string FormatForLog(string[] msgs, string status)
        {
            string formattedMsg = "";
            if(status == "START")
            {
                formattedMsg = "[SERVER STARTED]: ";
                foreach(string msg in msgs)
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
            else
            {
                formattedMsg = FormatForLog("Unknown Operation", status);
            }

            return formattedMsg;
        }

        static public string FormatForLog(string msg, string status)
        {
            string formattedMsg = "";
            if (status == "STOP") // if the server is given a stop request. This is unimplemented.
            {
                formattedMsg = "[SERVER STOPPED]: ";
                formattedMsg += msg;
            }
            else if (status == "RECEIVE")
            {
                formattedMsg = "[RECEIVED]: ";
                try
                {
                    int index = msg.IndexOf("HTTP");
                    msg = msg.Substring(0, index - 1);
                }
                catch(Exception e) // if a weird response comes in where the verb and path can;t be parsed, format an exception log.
                {
                    string tmp = FormatForLog("Bad Request Received: ", "EXCEPTION");
                    msg = tmp + msg;
                }
                formattedMsg += msg;
            }
            else if (status == "RESPONSE")
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
                    int codeStart = msg.IndexOf(' ') + 1; // find index of status code beginning
                    int codeEnd = msg.IndexOf("/r/n") - 1; // find status code end
                    msg = msg.Substring(codeStart, codeEnd - codeStart); // extract the status code
                }
                formattedMsg += msg;
            }
            else if(status == "EXCEPTION")
            {
                formattedMsg = "[EXCEPTION]: ";
                formattedMsg += msg;
            }
            else
            {
                formattedMsg = "[UNKNOWN]: ";
                formattedMsg += status + " - ";
                formattedMsg += msg;
            }
            return formattedMsg;
        }
    }
}
