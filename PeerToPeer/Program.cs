using System;
using System.Threading;
using System.Threading.Tasks;

namespace PeerToPeer
{
   class Program
   {
      static void Main(string[] args)
      {
         ConsoleWrapper console = new ConsoleWrapper();
         IOArea io = new IOArea(console);
         string input = console.Input(0, 0, "Enter username:this port >");
         var info = input.Split(':');
         string username = info[0];
         int port = Int32.Parse(info[1]);

         var serverResetEvent = new AutoResetEvent(false);
         var server = new PeerServer(serverResetEvent, port);
         server.Subscribe(new StringObserver(console));

         try
         {
            server.StartListening();
            Task.Factory.StartNew(
               () => server.WaitForConnection()
            );

            string userInput;
            int y = Console.WindowHeight - 1;
            do
            {
               Console.SetCursorPosition(0, y);
               Console.Write("CMD>");
               userInput = Console.ReadLine();
            } while (userInput != "QUIT");
         }
         catch(Exception ex)
         {
            Console.WriteLine($"Exception : {ex}");
         }
      }

   }
}
