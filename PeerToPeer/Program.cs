using System;
using System.Threading;
using System.Threading.Tasks;

namespace PeerToPeer
{
   class Program
   {
      static void Main(string[] args)
      {
         AutoResetEvent autoResetEvent = new AutoResetEvent(false);
         PeerServer server = new PeerServer(autoResetEvent);
         server.Subscribe(new ServerObserver());

         try
         {
            server.StartListening();
            Task.Factory.StartNew(
               () => server.WaitForConnection()
            );
            autoResetEvent.WaitOne();
         }
         catch(Exception ex)
         {
            Console.WriteLine($"Exception : {ex}");
         }

      }
   }
}
