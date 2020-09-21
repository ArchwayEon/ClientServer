using PeerToPeer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private Queue<Guid> _recentMessages;
        public string User { get; private set; }
        public bool Debug { get; private set; } = false;


        public PeerToPeerForm()
        {
            InitializeComponent();
            _serverResetEvent = new AutoResetEvent(false);
            _clients = new ConcurrentBag<PeerClient>();
            _recentMessages = new Queue<Guid>(64);
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

        public void HandleMessage(string message)
        {
            // <Recipient>|<Guid>|<Message String>
            // Break apart incoming message into its parts
            string[] tokens = message.Split('|', 4);
            string sender = tokens[0];
            string recipient = tokens[1];
            Guid messageGuid = Guid.Parse(tokens[2]);
            string messageText = tokens[3];

            if (_recentMessages.Contains(messageGuid))
                return;

            if (_recentMessages.Count == 64)
                _recentMessages.Dequeue();

            _recentMessages.Enqueue(messageGuid);

            if (String.IsNullOrWhiteSpace(recipient))
            {
                AppendTextBox($"{sender}> {messageText}");
            }
            else if (recipient == User)
            {
                AppendTextBox($"{sender}->{recipient}> {messageText}");
            }
            else
            {
                Broadcast(message);
            }

        }

        private void ProcessCommand(string command)
        {
            var length = command.Length;
            var index = command.IndexOf(' ');
            var cmd = String.Empty;

            if (index > 0)
                cmd = command.Substring(0, index).ToLower();
            else
                cmd = command;

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
                    ProcessChat(parameters);
                    break;
                case "debug":
                    ProcessDebug(parameters);
                    break;
            }

        }

        private void ProcessDebug(string parameters)
        {
            switch (parameters.ToLower())
            {
                case "t":
                case "y":
                case "true":
                case "1":
                case "yes":
                    Debug = true;
                    break;
                case "f":
                case "n":
                case "false":
                case "0":
                case "no":
                    Debug = false;
                    break;
                default:
                    Debug = !Debug;
                    break;
            }

            switch (Debug)
            {
                case true:
                    AppendTextBox("Debug enabled");
                    break;
                case false:
                    AppendTextBox("Debug disabled");
                    break;
            }
        }

        private void ProcessSend(string parameters)
        {
            AppendTextBox($"{User}> {parameters}");
            Broadcast(FormatMessage(parameters));
        }

        private void ProcessChat(string parameters)
        {
            var length = parameters.Length;
            var index = parameters.IndexOf(' ');
            var recipient = parameters.Substring(0, index).ToLower();
            var messageText = parameters[(index + 1)..length];

            AppendTextBox($"{User}->{recipient}> {messageText}");
            Broadcast(FormatMessage(recipient, messageText));
        }

        private void Broadcast(string message)
        {
            Task.Factory.StartNew(
                () =>
                {
                    foreach (var client in _clients)
                    {
                        client.SendRequest(message);
                    }
                });
        }

        private void ProcessConnect(string parameters)
        {
            int port = Int32.Parse(parameters);
            var client = new PeerClient(this);
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

            User = user;
            _server = new PeerServer(_serverResetEvent, this, port);
            _server.Subscribe(new StringObserver(this));
            _server.StartListening();
            Task.Factory.StartNew(
               () => _server.WaitForConnection()
            );
        }

        public string FormatMessage(string recipient, string messageText)
        {
            return $"{User}|{recipient}|{Guid.NewGuid()}|{messageText}";
        }

        public string FormatMessage(string messageText)
        {
            return $"{User}||{Guid.NewGuid()}|{messageText}";
        }

        public void AppendTextBox(string message)
        {
            if (txtMain.InvokeRequired)
                txtMain.Invoke(new AppendTextBoxDelegate(AppendTextBox), new object[] { message });
            else
                txtMain.AppendText(message + Environment.NewLine);
        }
    }
}
