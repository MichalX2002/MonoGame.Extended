using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace MonoGame.Extended.Testing
{
    internal class ResourceRequest : IResponseStatus, IDisposable
    {
        private ResourceStream _stream;

        public long ContentLength { get { return _stream == null ? -1 : _stream.Length; } }
        public long BytesDownloaded { get { return _stream == null ? -1 : _stream.Position; } }

        public Uri Url { get; }
        public string Accept { get; }
        public Exception Fault { get; private set; }

        public bool IsDisposed { get; private set; }
        public bool IsFaulted { get; private set; }
        public bool IsComplete { get; private set; }
        public bool IsCanceled { get; private set; }

        public readonly OnResponseDelegate OnResponse;
        public readonly OnErrorDelegate OnError;

        public ResourceRequest(Uri uri, string accept, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            if (string.IsNullOrWhiteSpace(accept))
                throw new ArgumentNullException(accept);

            Url = uri ?? throw new ArgumentNullException(nameof(uri));
            Accept = accept;
            OnResponse = onResponse ?? throw new ArgumentNullException(nameof(onResponse));
            OnError = onError;
        }

        public void HandleOnResponse(FileStream stream, WebHeaderCollection headers)
        {
            if (IsCanceled)
                return;

            var resourceStream = new ResourceStream(stream, headers);
            HandleOnResponse(resourceStream);
        }

        public void HandleOnResponse(WebResponse response)
        {
            if (IsCanceled)
                return;

            var resourceStream = new ResourceStream(response);
            HandleOnResponse(resourceStream);
        }

        public void HandleOnResponse(ResourceStream stream)
        {
            try
            {
                _stream = stream;
                OnResponse.Invoke(Url, _stream);
                IsComplete = true;
            }
            catch (Exception exc)
            {
                IsFaulted = true;
                Fault = exc;
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        [DebuggerHidden]
        private void ValidateLifetime()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ResourceStream));
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

                IsDisposed = true;
            }
        }
    }
}
