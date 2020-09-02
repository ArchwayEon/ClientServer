﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ClientProgram
    {
        // 1. Allocate a buffer to store incoming data
        private readonly byte[] _bytes = new byte[1024];

        static void Main(string[] args)
        {
            var app = new ClientProgram();
            

            try
            {
                // 2. Establish a remote endpoint for the socket
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // 3. Create the socket
                var sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // 4. Connect the socket to the remote endpoint
                    sender.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                    Task receiveResponse = Task.Run(() => app.ReceiveResponse(sender));

                    string message = "";
                    string userInput = "";
                    do
                    {
                        userInput = app.GetUserInput();
                        switch (userInput)
                        {
                            case "1":
                                message = "View<EOF>";
                                break;
                            case "E":
                                message = "Exit<EOF>";
                                break;
                        }

                        // 5. Encode the data to be sent
                        byte[] msg = Encoding.ASCII.GetBytes(message);

                        // 6. Send the data through the socket
                        int bytesSent = sender.Send(msg);

                    } while (userInput != "E");
                    // 9. Close the socket
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected Exception: {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}", e.ToString());
            }
        } // Main

        private void ReceiveResponse(Socket sender) {
            string response;
            do
            {
                int bytesRec = sender.Receive(_bytes);
                response = Encoding.ASCII.GetString(_bytes, 0, bytesRec);
                Console.WriteLine($"\n{response}");
            } while (response != "Exit");
        }

        private string GetUserInput()
        {
            string userInput;
            do
            {
                Console.WriteLine("======================");
                Console.WriteLine("1. View Map           ");
                Console.WriteLine("E. Exit               ");
                Console.WriteLine("======================");
                Console.Write("Make a choice:");
                userInput = Console.ReadLine();
            } while (userInput != "1" && userInput != "E");
            return userInput;
        }
    }
}