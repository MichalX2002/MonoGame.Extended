using System;

namespace MonoGame.Extended.Testing
{
    public class ResourceManager : ResourceRequester
    {
        public ResourceDownloader Downloader { get; }
        public ResourceCache Cache { get; }

        public ResourceManager(string cachePath) : base(workers: 1)
        {
            Downloader = new ResourceDownloader();
            Cache = new ResourceCache(cachePath);
        }

        protected override void OnStart()
        {
            Downloader.Start();
            Cache.Start();
        }

        public override void OnRequest(ResourceRequest resourceRequest)
        {
            Cache.OnRequest(resourceRequest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Downloader.Dispose();
                Cache.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}