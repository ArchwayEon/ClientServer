using System;

namespace PeerToPeer
{
    public class StringObserver : IObserver<string>
    {
        private IDisposable _unsubscriber;
        private readonly ConsoleWrapper _console;
        private readonly IOArea _messageArea;
        private int _defaultX, _defaultY;

        public StringObserver(ConsoleWrapper console)
        {
            _console = console;
            _messageArea = new IOArea(_console);
        }

        public virtual void Subscribe(IObservable<string> provider)
        {
            _unsubscriber = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }

        public void OnCompleted()
        {
            int row = Console.WindowHeight;
            _messageArea.Show(0, row, "The server has terminated.");
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(string value)
        {
            int row = 0, col = 0;
            var items = value.Split('|');
            if (items.Length > 1)
            {
                var coords = items[1].Split(',');
                if (coords.Length > 1)
                {
                    col = Int32.Parse(coords[0]);
                    row = Int32.Parse(coords[1]);
                }
                else
                {
                    row = Int32.Parse(coords[0]);
                }
            }
            _messageArea.Show(col, row, items[0]);
        }

        public void SetDefaultPosition(int x, int y)
        {
            _defaultX = x;
            _defaultY = y;
        }
    }
}
