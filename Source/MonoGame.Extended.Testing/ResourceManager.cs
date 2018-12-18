using System;

namespace MonoGame.Extended.Testing
{
    public class ResourceManager : IResourceRequester
    {
        public bool IsDisposed => Downloader.IsDisposed;
        public bool IsRunning => Downloader.IsRunning;

        public ResourceDownloader Downloader { get; }
        public ResourceCache Cache { get; }

        public ResourceManager(string cachePath)
        {
            Downloader = new ResourceDownloader();
            Cache = new ResourceCache(cachePath);
        }

        public void Start()
        {
            Downloader.Start();
            Cache.Start();
        }

        public IResponseStatus Request(string uri, string accept, bool prioritized, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            var cachedResponse = Cache.Request(uri, accept, prioritized, onResponse, onError);
            if (cachedResponse.IsNotFound)
            {

            }

            return Downloader.Request(uri, accept, prioritized, onResponse, onError);
        }

        public IResponseStatus Request(Uri uri, string accept, bool prioritized, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            Cache.Request(uri, accept, prioritized, onResponse, onError);

            return Downloader.Request(uri, accept, prioritized, onResponse, onError);
        }

        public void Dispose()
        {
            Downloader.Dispose();
            Cache.Dispose();
        }

        public class Worker
        {
            public int ID { get; }
            public IResponseStatus CurrentRequest { get; internal set; }

            internal Worker(int id)
            {
                ID = id;
            }
        }
    }
}