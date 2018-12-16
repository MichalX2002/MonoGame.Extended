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
        private AutoResetEvent _requestEvent;
        private ConcurrentQueue<ResourceRequest> _requests;
        private ConcurrentQueue<ResourceRequest> _priorityRequests;
        private ConcurrentDictionary<Uri, ResourceRequest> _responses;

        private Thread[] _threads;
        private DownloadWorker[] _downloadWorkers;
        
        public bool IsDisposed { get; private set; }
        public bool IsRunning { get; private set; }
        public ReadOnlyCollection<DownloadWorker> Threads { get; }

        public ResourceDownloader()
        {
            _requestEvent = new AutoResetEvent(false);
            _requests = new ConcurrentQueue<ResourceRequest>();
            _priorityRequests = new ConcurrentQueue<ResourceRequest>();
            _responses = new ConcurrentDictionary<Uri, ResourceRequest>();

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

        public IResponseStatus Request(string uri, string accept, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            return Request(new Uri(uri), accept, onResponse, onError);
        }

        public IResponseStatus Request(Uri uri, string accept, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            var request = new ResourceRequest(uri, accept, onResponse, onError);
            _requests.Enqueue(request);
            _requestEvent.Set();
            return request;
        }

        private bool DequeueRequest(out ResourceRequest request)
        {
            if (_priorityRequests.TryDequeue(out request))
                return true;

            if (_requests.TryDequeue(out request))
                return true;

            return false;
        }

        private void DownloadWorkerThread(object obj)
        {
            var workerData = obj as DownloadWorkerData;
            while (IsRunning)
            {
                if (!_requestEvent.WaitOne(10))
                    continue;

                while (DequeueRequest(out ResourceRequest resourceRequest))
                {
                    var url = resourceRequest.Url;
                    _responses.TryAdd(url, resourceRequest);
                    workerData.CurrentRequest = resourceRequest;

                    try
                    {
                        var request = WebRequest.CreateHttp(url);
                        request.Accept = resourceRequest.Accept;

                        using (var response = request.GetResponse())
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
                IsRunning = false;

                if (_responses != null)
                {
                    foreach (var response in _responses)
                        response.Value.Dispose();
                }

                if (_threads != null)
                {
                    for (int i = 0; i < _threads.Length; i++)
                        _threads[i].Join();
                    _threads = null;
                }

                _requestEvent?.Dispose();
                _responses = null;

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
}