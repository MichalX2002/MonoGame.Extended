using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;

namespace MonoGame.Extended.Testing
{
    public delegate void OnErrorDelegate(Uri uri, Exception exception);
    public delegate void OnResponseDelegate(Uri uri, ResourceStream stream);

    public abstract class ResourceRequester : IDisposable
    {
        private AutoResetEvent _requestEvent;
        private ConcurrentQueue<ResourceRequest> _requests;
        private ConcurrentQueue<ResourceRequest> _priorityRequests;
        private ConcurrentDictionary<Uri, ResourceRequest> _responses;

        private Thread[] _threads;
        private Worker[] _workers;

        public bool IsDisposed { get; private set; }
        public bool IsRunning { get; private set; }

        public ReadOnlyCollection<Worker> Workers { get; }

        public ResourceRequester(int workers)
        {
            _requestEvent = new AutoResetEvent(false);
            _requests = new ConcurrentQueue<ResourceRequest>();
            _priorityRequests = new ConcurrentQueue<ResourceRequest>();
            _responses = new ConcurrentDictionary<Uri, ResourceRequest>();

            _threads = new Thread[workers];
            _workers = new Worker[_threads.Length];
            Workers = new ReadOnlyCollection<Worker>(_workers);

            for (int i = 0; i < _threads.Length; i++)
            {
                int id = i + 1;
                _workers[i] = new Worker(id);
                _threads[i] = new Thread(WorkerThread)
                {
                    Name = GetType().Name + " Worker " + id
                };
            }
        }

        public void Start()
        {
            if (IsRunning)
                return;

            for (int i = 0; i < _threads.Length; i++)
            {
                Worker worker = _workers[i];
                _threads[i].Start(worker);
            }

            OnStart();
            IsRunning = true;
        }

        protected virtual void OnStart()
        {
        }

        public abstract void OnRequest(ResourceRequest resourceRequest);

        public ResourceRequest Request(Uri uri, string accept, bool prioritized, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            var request = new ResourceRequest(uri, accept, onResponse, onError);
            if (prioritized)
                _priorityRequests.Enqueue(request);
            else
                _requests.Enqueue(request);
            _requestEvent.Set();
            return request;
        }

        private bool DequeueRequest(out ResourceRequest request)
        {
            if (IsRunning)
            {
                if (_priorityRequests.TryDequeue(out request))
                    return true;

                if (_requests.TryDequeue(out request))
                    return true;
            }

            request = null;
            return false;
        }

        private void WorkerThread(object state)
        {
            var worker = state as Worker;
            while (IsRunning)
            {
                if (!_requestEvent.WaitOne(10))
                    continue;

                while (DequeueRequest(out ResourceRequest request))
                {
                    var uri = request.Uri;
                    _responses.TryAdd(uri, request);
                    worker.CurrentRequest = request;

                    try
                    {
                        OnRequest(request);
                    }
                    catch (Exception exc)
                    {
                        request.OnError?.Invoke(uri, exc);
                    }

                    worker.CurrentRequest = null;
                    request.Dispose();
                    _responses.TryRemove(uri, out var finishedRequest);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (IsRunning)
                {
                    if (_threads != null)
                    {
                        for (int i = 0; i < _threads.Length; i++)
                            _threads[i].Join();
                        _threads = null;
                    }
                    
                    if (_responses != null)
                    {
                        foreach (var response in _responses)
                            response.Value.Dispose();
                        _responses = null;
                    }

                    if (_requestEvent != null)
                    {
                        _requestEvent.Dispose();
                        _requestEvent = null;
                    }

                    IsRunning = false;
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        
        public class Worker
        {
            public int ID { get; }
            public ResourceRequest CurrentRequest { get; internal set; }

            internal Worker(int id)
            {
                ID = id;
            }
        }
    }
}