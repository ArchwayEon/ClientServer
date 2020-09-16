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
            serverResetEvent.WaitOne();
         }
         catch(Exception ex)
         {
            Console.WriteLine($"Exception : {ex}");
         }

      }
   }
}
