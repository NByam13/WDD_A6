using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyOwnWebServer
{
    public class WebServer
    {
        private const int kReqArgNum = 3;
        private const int kMaxSplitArgs = 6;
        private const int kMaxUnkownArgs = 3;
        private const int kHelpCode = 1;
        private const int kBadArgCount = -1;
        private const int kUnknownArg = -2;
        private const int kProblem = -3;

        public bool run = true;
        public string DataPath { get; set; }
        public int Port { get; set; }
        public IPAddress ServerIP { get; set; }


        /////////////////////////////////////////
        // Method       : WebServer (ctor)
        // Description  : Constructor for the webserver class. The constructor calls on a method to set the command line
        //              : arguments that were passed in to the right properties so that the server can start.
        // Parameters   : string[] args : the string array of arguments
        // Returns      : N/A
        /////////////////////////////////////////
        public WebServer(string[] args)
        {
            SetProperties(args);
        }


        /////////////////////////////////////////
        // Method       : main
        // Description  : In the main the program checks to see if the number of cmd line arguments is correct, if the user
        //              : has asked for help, and how to proceed if the right arguments have been given.
        // Parameters   : string[] args : command line arguments used to start the server, or give the user a help text
        // Returns      : N/A
        /////////////////////////////////////////
        static void Main(string[] args)
        {
            if(args.Length != kReqArgNum) // if the return code of the check args method is not 0, that means there are more or less
            {                           // than 3 args, direct the user to the help feature and exit with -1.

                Logger.Log(Logger.FormatForLog("Server Unable to Start, Invalid Argument Count", "EXCEPTION")); //#### Sean's spec####
                Environment.Exit(kBadArgCount);
            }
            else // everything is good to go here, we have the right number of args so far, just need to check them
            {
                string[] argArray = ParseArgs(args); // check the args and split them with parse
                if(argArray.Length <= kMaxUnkownArgs) // if the amount of args that comes back is 3 or less, we know one or more was not
                {                                     // recognized as valid

                    Logger.Log(Logger.FormatForLog("Unknown Arguments Gathered From Cmd Line","EXCEPTION"));
                    Environment.Exit(kUnknownArg); // exit with the proper error code
                }
                else if(argArray.Length == kMaxSplitArgs) // check to make sure we have 6 elements after splitting, if we don't 
                {                                         // then something went wrong
                    WebServer Server = new WebServer(argArray);
                    Logger.Log(Logger.FormatForLog(argArray, "START"));
                    Server.StartServer();
                    Logger.Log(Logger.FormatForLog("Server Shut Down","STOP"));
                }
                else
                {
                    Logger.Log(Logger.FormatForLog("The Number of Arguments Was Invalid","Argument Count Error"));
                    Environment.Exit(kProblem);
                }
            }
            
        }


        /////////////////////////////////////////
        // Method       : StartServer
        // Description  : A method to create and start a listener based on the Server's given IP and Port. The listener then waits to
        //              : accept a tcp client.
        // Parameters   : N/A
        // Returns      : N/A
        /////////////////////////////////////////
        public void StartServer()
        {
            TcpListener listener = new TcpListener(ServerIP, Port); // create a listener on the given IP and Port
            listener.Start(); // start the listener
            while(run) // wait for incoming connection requests until the run boolean is set to false. 
            {
                if(!listener.Pending()) // if there is not a connection pending continue to next iteration, therefore no blocking occurs
                {
                    continue;
                }
                else
                {
                    TcpClient client = listener.AcceptTcpClient(); // when a connection is pending, connect to client
                    HandleClient(client); // jump to a method to handle the client.
                    client.Close(); // Might not need this here if we close in Handle Client.
                }
            }
            // stop the listener after run is set to false.
            listener.Stop();
        }


        /////////////////////////////////////////
        // Method       : HandleClient
        // Description  :
        // Parameters   : TcpClient client : the client connection
        // Returns      : N/A
        /////////////////////////////////////////
        public void HandleClient(TcpClient client)
        {
            // get incoming communication
            var stream = client.GetStream();

        }


        /////////////////////////////////////////
        // Method       : ParseArgs
        // Description  : A method that checks for unknown arguments, and if there are none, breaks up the arguments into a list
        //              : containing the key and the value.
        // Parameters   : string[] args : the cmd line arguments
        // Returns      : string[] err : if unknown commands are found in cmd line args, send back a list of the unknown commands
        //              : string[] argArray : a complete list of valid cmd line arguments broken into alternating keys and values
        /////////////////////////////////////////
        static public string[] ParseArgs(string[] args)
        {
            string[] err = new string[3];
            int count = 0;
            foreach(string arg in args) // this will iterate through all args, they can be in any order.
            {                           // if an argument is found that is not recognized, it will be returned as a list
                if(arg.StartsWith("-webRoot=")) // of unknown arguments.
                {
                    continue;
                }
                else if(arg.StartsWith("-webIP="))
                {
                    continue;
                }
                else if(arg.StartsWith("-webPort="))
                {
                    continue;
                }
                else // if the current arg is unrecognized, add it to the error list.
                {
                    err[count] = arg;
                    count++;
                }
            }
            if(err[0] != null) // if the error list has anything in it, the first index will not be null
            {
                return err; // return the unknown args
            }

            string[] argArray = new string[kMaxSplitArgs]; // create an array of size 6 to hold the valid keys and values
            count = 0;
            char splitChar = '='; // we will split on = because that divides the key and value for all args
            foreach (string arg in args) // iterate through all args
            {
                string[] tmp;
                int i = 0;
                tmp = arg.Split(splitChar); // split on =
                argArray[count] = tmp[i]; // add key to array
                count++;
                i++;
                argArray[count] = tmp[i]; // add value to array
                count++;
            }
            return argArray; // return key value array
        }


        /////////////////////////////////////////
        // Method       : SetProperties
        // Description  : A method to set the given cmd line arguments to the right properties. It iterates through an array attempting to
        //              : to parse the right info out. By doing it this way, the properties can be set properly regardless of the order
        //              : the cmd line arguments were given.
        // Parameters   : string[] args : the array containing the split up cmd line arguments
        // Returns      : N/A
        /////////////////////////////////////////
        private void SetProperties(string[] args)
        {
            for(int i = 0; i < args.Length; i++) // loop over entire array
            {
                if(i%2 != 0) // the pertinent info will be at indeces 1, 3, 5
                {
                    if (int.TryParse(args[i], out int tmpPort))
                    {
                        Port = tmpPort; // set port if the current value is an integer value
                    }
                    else if (IPAddress.TryParse(args[i], out IPAddress tmpIP))
                    {
                        ServerIP = tmpIP; // set ServerIP if the current value is an IPAddress type
                    }
                    else
                    {
                        DataPath = args[i]; // if the value is neither an Int nor an IPAddress type, then it is the data path.
                    }
                }
            }
        }
    }
}
