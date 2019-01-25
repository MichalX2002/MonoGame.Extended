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
        private ConcurrentQueue<ResourceResponse> _requests;
        private ConcurrentQueue<ResourceResponse> _priorityRequests;
        private ConcurrentDictionary<Uri, ResourceResponse> _responses;

        private Thread[] _threads;
        private Worker[] _workers;

        public bool IsDisposed { get; private set; }
        public bool IsRunning { get; private set; }

        public ReadOnlyCollection<Worker> Workers { get; }

        public ResourceRequester(int workers)
        {
            _requestEvent = new AutoResetEvent(false);
            _requests = new ConcurrentQueue<ResourceResponse>();
            _priorityRequests = new ConcurrentQueue<ResourceResponse>();
            _responses = new ConcurrentDictionary<Uri, ResourceResponse>();

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
            IsRunning = true;

            for (int i = 0; i < _threads.Length; i++)
            {
                Worker worker = _workers[i];
                _threads[i].Start(worker);
            }

            OnStart();
        }

        protected virtual void OnStart()
        {
        }

        public abstract RequestStatus ProcessRequest(ResourceResponse request, bool prioritized);

        public ResourceResponse Request(Uri uri, string accept, bool prioritized, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            var request = new ResourceResponse(uri, accept, onResponse, onError);
            Request(request, prioritized);
            return request;
        }

        public void Request(ResourceResponse request, bool prioritized)
        {
            request.Status = RequestStatus.Queued;
            if (prioritized)
                _priorityRequests.Enqueue(request);
            else
                _requests.Enqueue(request);
            _requestEvent.Set();
        }

        private bool DequeueRequest(out ResourceResponse request, out bool prioritized)
        {
            if (IsRunning)
            {
                if (_priorityRequests.TryDequeue(out request))
                {
                    prioritized = true;
                    return true;
                }

                if (_requests.TryDequeue(out request))
                {
                    prioritized = false;
                    return true;
                }
            }

            request = null;
            prioritized = false;
            return false;
        }

        private void WorkerThread(object state)
        {
            var worker = state as Worker;
            while (IsRunning)
            {
                if (!_requestEvent.WaitOne(10))
                    continue;
                
                while (DequeueRequest(out ResourceResponse request, out bool prioritized))
                {
                    var uri = request.Uri;
                    _responses.TryAdd(uri, request);
                    worker.CurrentRequest = request;

                    try
                    {
                        request.Status = RequestStatus.Processing;
                        request.Status = ProcessRequest(request, prioritized);
                    }
                    catch (Exception exc)
                    {
                        request.Fault = exc;
                        request.Status = RequestStatus.Faulted;
                        request.OnError?.Invoke(uri, exc);
                    }

                    request.OnCompletion();
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
                    IsRunning = false;

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
            public ResourceResponse CurrentRequest { get; internal set; }

            internal Worker(int id)
            {
                ID = id;
            }
        }
    }
}