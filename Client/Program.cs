using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
   class Program
   {
      static void Main(string[] args)
      {
         var app = new Program();
         // 1. Allocate a buffer to store incoming data
         byte[] bytes = new byte[1024];

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

                  // 7. Listen for the reponse (blocking call)
                  int bytesRec = sender.Receive(bytes);
                  // 8. Process the response
                  Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
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
