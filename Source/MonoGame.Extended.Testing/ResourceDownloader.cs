using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace MonoGame.Extended.Testing
{
    public class ResourceDownloader : IResourceRequester
    {
        private ConcurrentDictionary<Uri, ResourceRequest> _responses;
        private ConcurrentQueue<ResourceRequest> _requests;
        private AutoResetEvent _requestEvent;

        private Thread[] _threads;
        private DownloadWorker[] _downloadWorkers;
        
        public bool IsDisposed { get; private set; }
        public bool IsRunning { get; private set; }
        public ReadOnlyCollection<DownloadWorker> Threads { get; }

        public ResourceDownloader()
        {
            _responses = new ConcurrentDictionary<Uri, ResourceRequest>();
            _requests = new ConcurrentQueue<ResourceRequest>();
            _requestEvent = new AutoResetEvent(false);

            _threads = new Thread[1];
            _downloadWorkers = new DownloadWorker[_threads.Length];
            Threads = new ReadOnlyCollection<DownloadWorker>(_downloadWorkers);
            
            for (int i = 0; i < _threads.Length; i++)
            {
                int id = i + 1;
                _downloadWorkers[i] = new DownloadWorker(new DownloadWorkerData(id));
                _threads[i] = new Thread(DownloadWorkerThread)
                {
                    Name = "ResourceDownloader Thread " + id
                };
            }
        }

        public void Start()
        {
            if (IsRunning)
                return;
            IsRunning = true;

            for (int i = 0; i < _threads.Length; i++)
            {
                DownloadWorker worker = _downloadWorkers[i];
                _threads[i].Start(worker.Data);
            }
        }

        public IResponseStatus Request(string uri, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            return Request(new Uri(uri), onResponse, onError);
        }

        public IResponseStatus Request(Uri uri, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            var request = new ResourceRequest(uri, onResponse, onError);
            _requests.Enqueue(request);
            _requestEvent.Set();
            return request;
        }

        private void DownloadWorkerThread(object obj)
        {
            var workerData = obj as DownloadWorkerData;
            while (IsRunning)
            {
                if (!_requestEvent.WaitOne())
                    continue;

                while (_requests.TryDequeue(out ResourceRequest resourceRequest))
                {
                    var url = resourceRequest.Url;
                    _responses.TryAdd(url, resourceRequest);
                    workerData.CurrentRequest = resourceRequest;

                    try
                    {
                        using (var response = WebRequest.CreateHttp(url).GetResponse())
                            resourceRequest.HandleOnResponse(response);
                    }
                    catch (Exception exc)
                    {
                        resourceRequest.OnError?.Invoke(url, exc);
                    }

                    _responses.TryRemove(url, out var tmp);
                    workerData.CurrentRequest = null;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    IsRunning = false;
                    
                    foreach (var response in _responses)
                        response.Value.Dispose();
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public class DownloadWorker
        {
            internal DownloadWorkerData Data { get; }

            public int ID => Data.ID;
            public IResponseStatus CurrentRequest => Data.CurrentRequest;

            internal DownloadWorker(DownloadWorkerData data)
            {
                Data = data;
            }
        }

        internal class DownloadWorkerData
        {
            public int ID { get; }
            public IResponseStatus CurrentRequest { get; set; }

            public DownloadWorkerData(int id)
            {
                ID = id;
            }
        }

        private class ResourceRequest : IResponseStatus, IDisposable
        {
            private ResourceStream _stream;

            public long ContentLength { get { ValidateLifetime(); return _stream == null ? -1 : _stream.Length; } }
            public long BytesDownloaded { get { ValidateLifetime(); return _stream == null ? -1 : _stream.Position; } }

            public Uri Url { get; }
            public Exception Fault { get; private set; }

            public bool IsDisposed { get; }
            public bool IsFaulted { get; private set; }
            public bool IsComplete { get; private set; }
            public bool IsCanceled { get; private set; }

            public readonly OnResponseDelegate OnResponse;
            public readonly OnErrorDelegate OnError;

            public ResourceRequest(Uri uri, OnResponseDelegate onResponse, OnErrorDelegate onError)
            {
                Url = uri ?? throw new ArgumentNullException(nameof(uri));
                OnResponse = onResponse ?? throw new ArgumentNullException(nameof(onResponse));
                OnError = onError;
            }

            public void HandleOnResponse(WebResponse response)
            {
                if (IsCanceled)
                    return;

                try
                {
                    _stream = new ResourceStream(response);
                    OnResponse.Invoke(Url, _stream);
                    IsComplete = true;
                }
                catch (Exception exc)
                {
                    IsFaulted = true;
                    Fault = exc;
                    throw;
                }
            }

            [DebuggerHidden]
            private void ValidateLifetime()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(ResourceStream));
            }

            public void Dispose()
            {
                if (!IsComplete)
                    IsCanceled = true;

                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }
            }
        }
    }
}