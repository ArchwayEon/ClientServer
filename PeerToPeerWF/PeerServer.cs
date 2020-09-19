using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace PeerToPeer
{
   public class PeerServer : IObservable<string>
   {
      private ConcurrentBag<PeerClient> _peers;
      private readonly ConcurrentBag<IObserver<string>> _observers;
      private readonly AutoResetEvent _autoResetEvent;
      private IPHostEntry _ipHostInfo;
      private IPEndPoint _localEndPoint;
      private Socket _listener;
      private int _numberOfConnections;
      public int NumberOfConnections { get { return _numberOfConnections; } }

      public string UserName { get; set; }

      public int BackLog { get; set; } = 10;

      public bool TimeToExit { get; set; } = false;

      public IPAddress IPAddress { get; private set; }

      public int PortNumber { get; }

      public PeerServer(AutoResetEvent autoResetEvent, int portNumber = 11000)
      {
         _observers = new ConcurrentBag<IObserver<string>>();
         _autoResetEvent = autoResetEvent;
         PortNumber = portNumber;
         _numberOfConnections = 0;
         _peers = new ConcurrentBag<PeerClient>();
         SetUpLocalEndPoint();
      }

      private void SetUpLocalEndPoint()
      {
         _ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
         IPAddress = _ipHostInfo.AddressList[0];
         _localEndPoint = new IPEndPoint(IPAddress, PortNumber);
      }

      public void StartListening()
      {
         _listener = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
         _listener.Bind(_localEndPoint);
         _listener.Listen(BackLog);
      }

      public void WaitForConnection()
      {
         do
         {
            ReportMessage("Waiting for a connection...");
            Socket handler = _listener.Accept();
            Task.Factory.StartNew(
               () => HandleRequest(handler)
            );
         } while (!TimeToExit);
         _autoResetEvent.Set();
      }

      private void HandleRequest(Socket handler)
      {
         Interlocked.Increment(ref _numberOfConnections);
         ReportMessage($"Number of connections: {_numberOfConnections}");
         byte[] buffer = new byte[1024];
         string data;
         string request;
         do
         {
            data = "";
            // Process the connection to read the incoming data
            while (true)
            {
               int bytesRec = handler.Receive(buffer);
               data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
               int index = data.IndexOf("<EOF>");
               if (index > -1)
               {
                  request = data.Substring(0, index);
                  break;
               }
            }
            ReportMessage($"RECEIVED:{request}");
            Task.Run(
               () => ProcessRequest(handler, request)
            );
         } while (request != "Exit");

         Interlocked.Decrement(ref _numberOfConnections);
         ReportMessage($"Number of connections: {_numberOfConnections}");

         handler.Shutdown(SocketShutdown.Both);
         handler.Close();
      }

      private void ProcessRequest(Socket handler, string request)
      {
         var len = request.Length;
         var index = request.IndexOf(':');
         var left = request.Substring(0, index).ToLower();
         switch (left)
         {
            case "i am":
               var right = request[(index + 1)..len];
               var parameters = right.Split(',');
               var userName = parameters[0];
               var clientPort = Int32.Parse(parameters[1]);
               var remoteEP = handler.RemoteEndPoint as IPEndPoint;
               var ipAddress = remoteEP.Address;
               ReportMessage($"YOU ARE: {userName}, {clientPort}, {ipAddress}");
               var checkPeer = FindPeer(ipAddress, clientPort);
               if(checkPeer == null)
               {
                  ReportMessage($"{userName} is not yet connected");
                  var client = new PeerClient();
                  client.Subscribe(_observers.First());
                  client.UserName = userName;
                  ConnectToPeer(client, clientPort);
               }
               else
               {
                  ReportMessage($"{userName} is already connected");
               }
               break;
            default:
               byte[] msg = Encoding.ASCII.GetBytes(request);
               handler.Send(msg);
               break;
         }
      }

      public void SendToAllPeers(string message)
      {
         Task.Factory.StartNew(
            () =>
            {
               foreach (var peer in _peers)
               {
                  peer.SendRequest(message);
               }
            }
         );
      }

      public void ConnectToPeer(PeerClient peerClient, int port)
      {
         _peers.Add(peerClient);
         peerClient.SetUpRemoteEndPoint(IPAddress, port);
         peerClient.ConnectToRemoteEndPoint();
         peerClient.SendRequest($"I AM:{UserName},{PortNumber}");
         Task.Factory.StartNew(
            () =>
            {
               peerClient.ReceiveResponse();
            }
         );
      }

      private PeerClient FindPeer(IPAddress ipAddress, int port)
      {
         return _peers.FirstOrDefault(c =>
            c.EndPoint.Address.Equals(ipAddress) && c.EndPoint.Port == port);
      }

      public IDisposable Subscribe(IObserver<string> observer)
      {
         if (!_observers.Contains(observer))
            _observers.Add(observer);

         return new MessageUnsubscriber(_observers, observer);
      }

      private void ReportMessage(string message)
      {
         foreach (var observer in _observers)
         {
            observer.OnNext(message);
         }
      }
   }
}
