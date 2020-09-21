using PeerToPeerWF;
using System;
using System.Windows.Forms;

namespace PeerToPeer
{
    public class StringObserver : IObserver<string>
    {
        private IDisposable _unsubscriber;
        private readonly PeerToPeerForm _form;
        private readonly object _lockObject = new object();

        public StringObserver(PeerToPeerForm form)
        {
            _form = form;
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
            lock (_lockObject)
            {
                _form.AppendTextBox("The server has terminated.");
            }

        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(string value)
        {
            lock (_lockObject)
            {
                _form.AppendTextBox(value);
            }

        }

    }
}
