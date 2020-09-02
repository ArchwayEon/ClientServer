using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class ServerProgram
    {
        private int _numberOfConnections = 0;

        static void Main(string[] args)
        {
            // Allocate a buffer to store incoming data
            byte[] bytes = new byte[1024];
            string data;

            // Establish a local endpoint for the socket
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create the socket
            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Bind the socket to the local endpoint
                listener.Bind(localEndPoint);
                // Listen for incoming connections
                listener.Listen(10);

                // Loop
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Console.WriteLine(ipAddress.ToString());
                    Console.WriteLine(localEndPoint.ToString());

                    //    Listen for a connection (blocking call)
                    Socket handler = listener.Accept();

                    string request;
                    do
                    {
                        request = "";
                        data = "";
                        //    Process the connection to read the incoming data
                        while (true)
                        {
                            int bytesRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            int index = data.IndexOf("<EOF>");
                            if (index > -1)
                            {
                                request = data.Substring(0, index);
                                break;
                            }
                        }
                        //    Process the incoming data
                        Console.WriteLine("Request : {0}", request);
                        byte[] msg = Encoding.ASCII.GetBytes(request);

                        handler.Send(msg);
                    } while (request != "Exit");

                    // Close the connection
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    if (request == "Exit")
                    {
                        break;
                    }
                } // while(true)

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : {0}", e.ToString());
            }
            Console.WriteLine("\nPress ENTER to exit...");
            Console.Read();

        }

        private void HandleRequest(Socket handler) {
            _numberOfConnections++;
            byte[] bytes = new byte[1024];
            string data;
            string request;
            do
            {
                data = "";
                //    Process the connection to read the incoming data
                while (true)
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    int index = data.IndexOf("<EOF>");
                    if (index > -1)
                    {
                        request = data.Substring(0, index);
                        break;
                    }
                }
                //    Process the incoming data
                Console.WriteLine("Request : {0}", request);
                byte[] msg = Encoding.ASCII.GetBytes(request);

                handler.Send(msg);
            } while (request != "Exit");

            // Close the connection
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}
