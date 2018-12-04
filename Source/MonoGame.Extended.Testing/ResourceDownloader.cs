using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace MonoGame.Extended.Testing
{
    public class ResourceDownloader // : IDisposable implement this please
    {
        public delegate void ResourceReadyDelegate(string url, HttpWebResponse response);
        public delegate void ResourceFailedDelegate(string url);

        private bool _running = false;

        private object _requestLock;
        private Queue<ResourceRequest> _requestQueue;
        private Thread[] _downloaderThreads;

        public ResourceDownloader()
        {
            _requestLock = new object();
            _requestQueue = new Queue<ResourceRequest>();
            _downloaderThreads = new Thread[2];
        }

        public void Start()
        {
            _running = true;
            for (int i = 0; i < _downloaderThreads.Length; i++)
            {
                _downloaderThreads[i] = new Thread(RequestDownloaderThread)
                {
                    Name = "Resource Downloader " + (i + 1)
                };
                _downloaderThreads[i].Start();
            }
        }

        public void Unload()
        {
            _running = false;
            lock (_requestQueue)
                _requestQueue.Clear();
        }

        private void RequestDownloaderThread()
        {
            while (_running)
            {
                while (_requestQueue.Count > 0)
                {
                    ResourceRequest resourceRequest;
                    lock (_requestLock)
                    {
                        if (_requestQueue.Count > 0)
                            resourceRequest = _requestQueue.Dequeue();
                        else
                            continue;
                    }

                    try
                    {
                        string url = resourceRequest.Url;
                        var httpRequest = WebRequest.CreateHttp(url);
                        using (var response = httpRequest.GetResponse())
                            resourceRequest.OnResource.Invoke(url, response as HttpWebResponse);
                    }
                    catch (TimeoutException timeoutExc)
                    {
                        Console.WriteLine(resourceRequest.Url + ": Request timed out");
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(resourceRequest.Url + ": " + exc);
                    }
                }
                Thread.Sleep(10);
            }
        }

        public void Request(string url, ResourceReadyDelegate onResource)
        {
            CheckAccessibility();
            _requestQueue.Enqueue(new ResourceRequest(url, onResource));
        }

        [System.Diagnostics.DebuggerHidden]
        private void CheckAccessibility()
        {
            if (_running == false)
                throw new InvalidOperationException(
                    $"This downloader is currently not running. Call {nameof(Start)} to start it.");
        }

        struct ResourceRequest
        {
            public readonly string Url;
            public readonly ResourceReadyDelegate OnResource;

            public ResourceRequest(string url, ResourceReadyDelegate onTexture)
            {
                Url = url;
                OnResource = onTexture;
            }
        }
    }
}
