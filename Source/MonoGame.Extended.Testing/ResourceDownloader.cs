using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace MonoGame.Extended.Testing
{
    public class ResourceDownloader
    {
        public delegate void OnErrorDelegate(string url);
        public delegate void OnResponseDelegate(string url, HttpWebResponse response);

        private bool _running;

        private ConcurrentBag<WebResponse> _responses;
        private ConcurrentQueue<ResourceRequest> _requests;
        private Thread[] _requestDownloaders;

        public ResourceDownloader()
        {
            _responses = new ConcurrentBag<WebResponse>();
            _requests = new ConcurrentQueue<ResourceRequest>();
            _requestDownloaders = new Thread[1];

            for (int i = 0; i < _requestDownloaders.Length; i++)
            {
                _requestDownloaders[i] = new Thread(RequestDownloaderThread)
                {
                    Name = "Request Downloader " + (i + 1)
                };
            }
        }

        public void Start()
        {
            if (_running)
                return;
            _running = true;

            for (int i = 0; i < _requestDownloaders.Length; i++)
                _requestDownloaders[i].Start();
        }

        public void Unload()
        {
            if (_running)
            {
                _running = false;

                while (_responses.TryTake(out WebResponse response))
                    response.Dispose();
            }
        }

        public void Request(string url, OnResponseDelegate onResponse)
        {
            _requests.Enqueue(new ResourceRequest(url, onResponse));
        }

        private void RequestDownloaderThread()
        {
            while (_running)
            {
                while (_requests.Count > 0)
                {
                    if (!_running)
                        break;

                    if (!_requests.TryDequeue(out ResourceRequest request))
                        continue;

                    try
                    {
                        var contentRequest = WebRequest.CreateHttp(request.Url);
                        using (var response = contentRequest.GetResponse())
                        {
                            _responses.Add(response);
                            request.OnBody.Invoke(request.Url, response as HttpWebResponse);
                        }
                    }
                    catch (TimeoutException timeoutExc)
                    {
                        Console.WriteLine(request.Url + ": Request timed out");
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(request.Url + ": " + exc);
                    }
                }
                Thread.Sleep(10);
            }
        }
        
        struct ResourceRequest
        {
            public readonly string Url;
            public readonly OnResponseDelegate OnBody;

            public ResourceRequest(string url, OnResponseDelegate onTexture)
            {
                Url = url;
                OnBody = onTexture;
            }
        }
    }
}
