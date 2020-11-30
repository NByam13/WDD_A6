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
            else if(status == "RECEIVE")
            {
                formattedMsg = "[RECEIVED]: ";
                foreach(string msg in msgs)
                {
                    formattedMsg += msg + " ";
                }
            }
            else if(status == "RESPONSE")
            {
                formattedMsg = "[RESPONSE]: ";
                // #### Need to look at http header content ####
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
            if (status == "STOP")
            {
                formattedMsg = "[SERVER STOPPED]: ";
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
