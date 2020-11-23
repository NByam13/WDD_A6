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

        public WebServer(string[] args)
        {
            SetProperties(args);
        }

        static void Main(string[] args)
        {
            if(Help(args)) // if help is the first arg, then ignore any others, and print instructions to the cmd prompt
            {
                Environment.Exit(kHelpCode); // exit after printing instruction.
            }

            if(args.Length != kReqArgNum) // if the return code of the check args method is not 0, that means there are more or less
            {                           // than 3 args, direct the user to the help feature and exit with -1.
                Console.WriteLine("Incorrect Amount of Arguments: Try 'help' for instruction to start the Web Server.");
                Environment.Exit(kBadArgCount);
            }
            else // everything is good to go here, we have the right number of args so far, just need to check them
            {
                string[] argArray = ParseArgs(args); // check the args and split them with parse
                if(argArray.Length <= kMaxUnkownArgs) // if the amount of args that comes back is 3 or less, we know one or more was not
                {                                     // recognized as valid
                    foreach(string arg in argArray) // iterate through the error array, print the unknown commands to the screen
                    {
                        Console.WriteLine("Unknown Argument: {0}", arg);
                    }
                    Console.WriteLine("\nTry 'help' for instruction to start the Web Server.\n"); // recommend the help command
                    Environment.Exit(kUnknownArg); // exit with the proper error code
                }
                else if(argArray.Length == kMaxSplitArgs) // check to make sure we have 6 elements after splitting, if we don't 
                {                                         // then something went wrong
                    WebServer Server = new WebServer(argArray);
                    Server.StartServer();
                    Console.WriteLine("Server Shut Down.\n");
                }
                else
                {
                    Console.WriteLine("\nSomething went wrong. Please try starting the server again.\n");
                    Environment.Exit(kProblem);
                }
            }
            
        }


        public void StartServer()
        {
            TcpListener listener = new TcpListener(ServerIP, Port);
            listener.Start();

            while(run)
            {
                if(!listener.Pending())
                {
                    continue;
                }
                else
                {
                    TcpClient client = listener.AcceptTcpClient();
                    HandleClient(client);
                    client.Close(); // Might not need this here if we close in Handle Client.
                }
            }
            // stop the listener after run is set to false.
            listener.Stop();
        }


        public void HandleClient(TcpClient client)
        {
            //
        }



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
                else
                {
                    err[count] = arg;
                    count++;
                }
            }
            if(err[0] != null)
            {
                return err;
            }

            string[] argArray = new string[kMaxSplitArgs];
            count = 0;
            char splitChar = '=';
            foreach (string arg in args)
            {
                string[] tmp;
                int i = 0;
                tmp = arg.Split(splitChar);
                argArray[count] = tmp[i];
                count++;
                i++;
                argArray[count] = tmp[i];
                count++;
            }
            return argArray;
        }

        static public bool Help(string[] args)
        {
            if(args[0].ToLower() == "help")
            {
                Console.WriteLine("|--- 3 Arguments are necessary to start the Web Server ---|\n");
                Console.WriteLine("1. -webRoot=<path/to/web/data>");
                Console.WriteLine("2. -webIP=<Starting IP of Server>");
                Console.WriteLine("3. -webPort=<Starting Port of Server>");
                return true;
            }
            else { return false; }
        }

        public void SetProperties(string[] args)
        {
            for(int i = 0; i < kMaxSplitArgs; i++)
            {
                if(i%2 != 0) // the pertinent info will be at indeces 1, 3, 5
                {
                    if (int.TryParse(args[i], out int tmpPort))
                    {
                        Port = tmpPort;
                    }
                    else if (IPAddress.TryParse(args[i], out IPAddress tmpIP))
                    {
                        ServerIP = tmpIP;
                    }
                    else
                    {
                        DataPath = args[i];
                    }
                }
            }
        }
    }
}
