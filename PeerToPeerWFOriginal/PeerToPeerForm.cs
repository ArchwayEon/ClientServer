using PeerToPeer;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PeerToPeerWF
{
    public partial class PeerToPeerForm : Form
    {
        public delegate void AppendTextBoxDelegate(string message);
        private AutoResetEvent _serverResetEvent;
        private PeerServer _server;
        private ConcurrentBag<PeerClient> _clients;

        public PeerToPeerForm()
        {
            InitializeComponent();
            _serverResetEvent = new AutoResetEvent(false);
            _clients = new ConcurrentBag<PeerClient>();
        }

        public void AppendTextBox(string message)
        {
            if (txtMain.InvokeRequired)
            {
                var del = new AppendTextBoxDelegate(AppendTextBox);
                txtMain.Invoke(del, new object[] { message });
            }
            else
            {
                txtMain.AppendText(message + Environment.NewLine);
            }
        }

        private void PeerToPeerForm_Load(object sender, EventArgs e)
        {
        }

        private void btnSend_MouseClick(object sender, MouseEventArgs e)
        {
            SendCommand();
        }

        private void SendCommand()
        {
            var command = commandBox.Text;
            ProcessCommand(command);

            commandBox.Clear();
        }

        private void commandBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendCommand();
                
        }

        private void ProcessCommand(string command)
        {
            var length = command.Length;
            var index = command.IndexOf(' ');
            var cmd = command.Substring(0, index).ToLower();
            var parameters = command[(index + 1)..length];
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
                    ProcessSend(cmd);
                    break;
            }

        }

        private void ProcessSend(string parameters)
        {
            Task.Factory.StartNew(
               () =>
               {
                   foreach (var client in _clients)
                   {
                       client.SendRequest(parameters);
                   }
               }
            );
        }

        private void ProcessConnect(string parameters)
        {
            int port = Int32.Parse(parameters);
            var client = new PeerClient();
            _clients.Add(client);
            client.Subscribe(new StringObserver(this));
            client.SetUpRemoteEndPoint(_server.IPAddress, port);
            client.ConnectToRemoteEndPoint();
            Task.Factory.StartNew(
               () => client.ReceiveResponse()
            );
        }

        private void ProcessSet(string parameters)
        {
            var options = parameters.Split(':');

            string user = options[0];
            int port = Int32.Parse(options[1]);

            txtMain.Text += "User: " + user + Environment.NewLine;
            txtMain.Text += "Port: " + port + Environment.NewLine;

            _server = new PeerServer(_serverResetEvent, user, port);
            _server.Subscribe(new StringObserver(this));
            _server.StartListening();
            Task.Factory.StartNew(
               () => _server.WaitForConnection()
            );
        }
    }
}
