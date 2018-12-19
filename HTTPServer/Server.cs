using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(iep);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket newclint = serverSocket.Accept();
                //TODO: accept connections and start thread for each accepted connection.
                Thread newthread = new Thread(new ParameterizedThreadStart
                  (HandleConnection));

                newthread.Start(newclint);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket
            Socket client = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            client.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] data = new byte[1024];
                    int length = client.Receive(data);

                    // TODO: break the while loop if receivedLen==0
                    if (length == 0)
                    {
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    string requsets = Encoding.ASCII.GetString(data);
                    Request r = new Request(requsets);
                    // TODO: Call HandleRequest Method that returns the response
                    Response rs = this.HandleRequest(r);
                    // TODO: Send Response back to client
                    byte[] data2 = new byte[1024];
                    data2 = Encoding.ASCII.GetBytes(rs.ResponseString);
                    client.Send(data2);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);

                }
            }

            // TODO: close client socket
            client.Close();
        }

        Response HandleRequest(Request request)
        {
            string location = null;
            string contents = null;
            string type = "html";
            StatusCode c = StatusCode.OK;
            try
            {
                //TODO: check for bad request 
                bool t = request.ParseRequest();
                if (t == true)
                {
                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    string pass = Configuration.RootPath + request.relativeURI;
                    //TODO: check for redirect
                    string loc = GetRedirectionPagePathIFExist(request.relativeURI);
                    if (loc != string.Empty)
                    {
                        c = StatusCode.Redirect;
                        location = loc;
                    }
                    else
                    {
                        loc = request.relativeURI;
                    }
                    //TODO: check file exists
                    //TODO: read the physical file
                    contents = LoadDefaultPage(loc);
                    if (contents == string.Empty)
                    {
                        c = StatusCode.NotFound;
                        string filePath = Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName);
                        contents = File.ReadAllText(@filePath);
                    }
                }
                else
                {
                    string filePath = Path.Combine(Configuration.RootPath, Configuration.BadRequestDefaultPageName);
                    c = StatusCode.BadRequest;
                    contents = File.ReadAllText(@filePath);
                }
                // Create OK response
                Response s = new Response(c, type, contents, location);
                return s;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. **
                string filePath = Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName);
                c = StatusCode.InternalServerError;
                contents = File.ReadAllText(@filePath);
                Logger.LogException(ex);
                Response s = new Response(c, type, contents, location);
                return s;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string location;
            if (Configuration.RedirectionRules.TryGetValue(relativePath, out location))
            {

            }
            else
            {
                location = string.Empty;

            }
            return location;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(@filePath))
            {
                /*log execption*/
                return string.Empty;
            }

            // else read file and return its content
            else
            {
                string contents = File.ReadAllText(@filePath);
                return contents;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 

                // then fill Configuration.RedirectionRules dictionary 
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
