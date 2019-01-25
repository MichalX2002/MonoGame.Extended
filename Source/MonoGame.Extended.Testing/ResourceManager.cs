using System;

namespace MonoGame.Extended.Testing
{
    public class ResourceManager : ResourceRequester
    {
        public ResourceDownloader Downloader { get; }
        public ResourceCache Cache { get; }

        public ResourceManager(string cachePath) : base(workers: 2)
        {
            Downloader = new ResourceDownloader();
            Cache = new ResourceCache(cachePath);
        }

        protected override void OnStart()
        {
            Downloader.Start();
            Cache.Start();
        }

        public override RequestStatus ProcessRequest(ResourceResponse resourceRequest, bool prioritized)
        {
            //var cacheResult = Cache.ProcessRequest(resourceRequest)
            //if (cacheResult == RequestStatus.Complete)
            //{
            //
            //}

            Downloader.Request(resourceRequest, prioritized);
            resourceRequest.Wait();

            return resourceRequest.Status;
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