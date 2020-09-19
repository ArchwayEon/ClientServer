using PeerToPeer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PeerToPeerWF
{
   public partial class PeerToPeerForm : Form
   {
      private AutoResetEvent _serverResetEvent;
      private PeerServer _server;
      

      public PeerToPeerForm()
      {
         InitializeComponent();
         _serverResetEvent = new AutoResetEvent(false);
      }

      private void PeerToPeerForm_Load(object sender, EventArgs e)
      {
      }

      private void btnSend_MouseClick(object sender, MouseEventArgs e)
      {
         var command = commandBox.Text;
         ProcessCommand(command);
         
         commandBox.Clear();
      }

      private void ProcessCommand(string command)
      {
         var length = command.Length;
         var index = command.IndexOf(' ');
         var cmd = command.Substring(0, index).ToLower();
         var parameters = command[(index+1)..length];
         switch (cmd)
         {
            case "set":
               ProcessSet(parameters);
               break;
            case "connect":
               ProcessConnect(parameters);
               break;
            case "send":
               ProcessSend(parameters);
               break;
            case "chat":
               ProcessChat(command);
               break;
         }
         
      }

      private void ProcessChat(string command)
      {
         _server.SendToAllPeers(command);
      }

      private void ProcessSend(string parameters)
      {
         _server.SendToAllPeers(parameters);
      }

      private void ProcessConnect(string parameters)
      {
         int port = Int32.Parse(parameters);
         var peer = new PeerClient();
         peer.Subscribe(new StringObserver(txtMain));
         _server.ConnectToPeer(peer, port);
      }

      private void ProcessSet(string parameters)
      {
         var options = parameters.Split(':');
         txtMain.Text += "User: " + options[0] + Environment.NewLine;
         txtMain.Text += "Port: " + options[1] + Environment.NewLine;
         int port = Int32.Parse(options[1]);
         _server = new PeerServer(_serverResetEvent, port)
         {
            UserName = options[0]
         };
         _server.Subscribe(new StringObserver(txtMain));
         _server.StartListening();
         Task.Factory.StartNew(
            () => _server.WaitForConnection()
         );
      }
   }
}
