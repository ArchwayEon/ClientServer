﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PeerToPeer
{
    public class PeerClient : IObservable<string>
    {
        private readonly ConcurrentBag<IObserver<string>> _observers;
        private readonly byte[] _bytes = new byte[1024];
        private IPAddress _serverIpAddress;
        private int _serverPort;
        private IPEndPoint _remoteEP;
        private Socket _sender;

        public PeerClient()
        {
            _observers = new ConcurrentBag<IObserver<string>>();
            _serverIpAddress = null;
            _serverPort = 11000;
            _remoteEP = null;
        }

        public void SetUpRemoteEndPoint(IPAddress serverIpAddress, int serverPort)
        {
            _serverIpAddress = serverIpAddress;
            _serverPort = serverPort;
            _remoteEP = new IPEndPoint(_serverIpAddress, _serverPort);
        }

        public void ConnectToRemoteEndPoint()
        {
            _sender = new Socket(_serverIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sender.Connect(_remoteEP);
            ReportMessage($"Socket connected to {_sender.RemoteEndPoint}");
        }

        public void ReceiveResponse()
        {
            string response;
            do
            {
                int bytesRec = _sender.Receive(_bytes);
                response = Encoding.ASCII.GetString(_bytes, 0, bytesRec);
                ReportMessage($"{response}");
            } while (response != "Exit");
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
