using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
   class ClientProgram
   {
      private readonly byte[] _bytes = new byte[1024];
      private readonly ConsoleMenu _menu;
      private readonly MessageArea _clientMessageArea;
      private readonly MessageArea _serverMessageArea;
      private readonly MapArea _map;
      private readonly ConsoleWrapper _console = new ConsoleWrapper();

      public ClientProgram()
      {
         _menu = new ConsoleMenu(_console);
         _clientMessageArea = new MessageArea(_console);
         _serverMessageArea = new MessageArea(_console);
         _map = new MapArea(_console);
         SetUpMenu();
         _clientMessageArea.Line = 0;
         _serverMessageArea.Line = Console.WindowHeight - 4;
         _serverMessageArea.Prompt = "SERVER";
         _serverMessageArea.BorderCharacter = '#';
      }

      public static void Main(string[] args)
      {
         var app = new ClientProgram();
         var mw = new ClientMessageWriter();

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
               app.ShowClientMessage($"Socket connected to {sender.RemoteEndPoint}");

               Task.Run(
                  () =>
                  {
                     app.ShowMap();
                  }
               );

               Task receiveResponse = Task.Run(
                    () => app.ReceiveResponse(sender)
               );

               string userInput = "";
               do
               {
                  Task<string> sendRequest = new Task<string>(
                      () => app.SendRequest(sender)
                  );
                  sendRequest.Start();
                  userInput = sendRequest.Result;

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

      private string SendRequest(Socket sender)
      {
         string userInput;
         string message = "";

         do
         {
            userInput = GetUserInput(0, 3);
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
            sender.Send(msg);
         } while (userInput != "E");

         return userInput;
      }

      private void ReceiveResponse(Socket sender)
      {
         string response;
         do
         {
            int bytesRec = sender.Receive(_bytes);
            response = Encoding.ASCII.GetString(_bytes, 0, bytesRec);
            ShowServerMessage($"{response}");
         } while (response != "Exit");
      }

      private void ShowClientMessage(string message)
      {
         _clientMessageArea.Show(message);
         _menu.PositionCursor();
      }

      private void ShowServerMessage(string message)
      {
         _serverMessageArea.Show(message);
         _menu.PositionCursor();
      }

      private void SetUpMenu()
      {
         _menu.AddOption("1. View Map           ");
         _menu.AddOption("E. Exit               ");
      }

      private string GetUserInput(int x, int y)
      {
         string userInput;
         do
         {

            _menu.Show(x, y);
            _menu.PositionCursor();
            userInput = Console.ReadLine();
         } while (userInput != "1" && userInput != "E");
         return userInput;
      }

      private void ShowMap()
      {
         _map.Show();
      }
   }
}
