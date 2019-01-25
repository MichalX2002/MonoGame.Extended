using System;
using System.Diagnostics;
using System.Threading;

namespace MonoGame.Extended.Testing
{
    public partial class ResourceResponse : IDisposable
    {
        private ResourceStream _stream;
        private Exception _fault;
        private AutoResetEvent _event;

        public long ContentLength { get { return _stream == null ? -1 : _stream.Length; } }
        public long BytesDownloaded { get { return _stream == null ? -1 : _stream.Position; } }

        public Uri Uri { get; }
        public string Accept { get; }

        public bool IsDisposed { get; private set; }
        public RequestStatus Status { get; internal set; }
        public Exception Fault
        {
            get => _fault;
            set
            {
                _fault = value ?? throw new ArgumentNullException();
                Status = RequestStatus.Faulted;
                OnError?.Invoke(Uri, value);
            }
        }

        public OnResponseDelegate OnResponse { get; private set; }
        public OnErrorDelegate OnError { get; private set; }

        public ResourceResponse(Uri uri, string accept, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            if (string.IsNullOrWhiteSpace(accept))
                throw new ArgumentNullException(accept);

            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Accept = accept;
            OnResponse = onResponse ?? throw new ArgumentNullException(nameof(onResponse));
            OnError = onError;

            _event = new AutoResetEvent(false);
        }

        public bool Wait()
        {
            return _event.WaitOne();
        }

        public bool Wait(int millisecondsTimeout)
        {
            return _event.WaitOne(millisecondsTimeout);
        }

        public void OnResponseStream(ResourceStream stream)
        {
            AssertNotDisposed();

            _stream = stream;
            OnResponse.Invoke(Uri, _stream);
        }

        internal void OnCompletion()
        {
            _event.Set();
        }

        [DebuggerHidden]
        private void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("this");
        }

        public void Cancel()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                if (Status != RequestStatus.Complete)
                    Status = RequestStatus.Canceled;

                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }

                OnResponse = null;
                OnError = null;

                IsDisposed = true;
            }
        }
    }
}
