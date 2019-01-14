using System;
using System.Diagnostics;
using System.Net;

namespace MonoGame.Extended.Testing
{
    public partial class ResourceRequest : IDisposable
    {
        public const string EXCEPTION_CANCELED = "This request was canceled.";

        private ResourceStream _stream;
        private Exception _fault;

        public long ContentLength { get { return _stream == null ? -1 : _stream.Length; } }
        public long BytesDownloaded { get { return _stream == null ? -1 : _stream.Position; } }

        public Uri Uri { get; }
        public string Accept { get; }
        public WebExceptionStatus FaultStatus { get; private set; }
        public Exception Fault
        {
            get => _fault;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                IsFaulted = true;
                OnError?.Invoke(Uri, value);
            }
        }

        public bool IsDisposed { get; private set; }
        public bool IsFaulted { get; private set; }
        public bool IsComplete { get; private set; }
        public bool IsCanceled { get; private set; }
        public bool IsNotFound { get; private set; }

        public OnResponseDelegate OnResponse { get; private set; }
        public OnErrorDelegate OnError { get; private set; }

        public ResourceRequest(Uri uri, string accept, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            if (string.IsNullOrWhiteSpace(accept))
                throw new ArgumentNullException(accept);

            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Accept = accept;
            OnResponse = onResponse ?? throw new ArgumentNullException(nameof(onResponse));
            OnError = onError;
        }

        internal void SetNotFound(WebExceptionStatus status)
        {
            FaultStatus = status;
            IsComplete = true;
            IsNotFound = true;
            IsFaulted = true;
        }

        public void HandleOnResponse(ResourceStream stream)
        {
            if (IsCanceled)
            {
                FaultStatus = WebExceptionStatus.RequestCanceled;
                return;
            }

            try
            {
                AssertNotDisposed();

                _stream = stream;
                OnResponse.Invoke(Uri, _stream);
                IsComplete = true;
            }
            catch (Exception exc)
            {
                IsFaulted = true;
                Fault = exc;
                FaultStatus = WebExceptionStatus.UnknownError;
            }
            finally
            {
                Dispose();
            }
        }

        [DebuggerHidden]
        private bool AssertNotDisposed()
        {
            return !IsDisposed;
        }

        [DebuggerHidden]
        private bool AssertNotCanceled()
        {
            FaultStatus = WebExceptionStatus.RequestCanceled;
            return !IsCanceled;
        }

        public void Cancel()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                if (!IsComplete)
                    IsCanceled = true;

                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }

                OnResponse = null;

                IsDisposed = true;
            }
        }
    }
}
