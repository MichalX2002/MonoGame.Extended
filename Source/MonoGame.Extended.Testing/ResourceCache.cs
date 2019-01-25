using System;
using System.IO;
using System.Net;

namespace MonoGame.Extended.Testing
{
    public class ResourceCache : ResourceRequester
    {
        public DirectoryInfo CacheDirectory { get; }
        
        public ResourceCache(DirectoryInfo directory) : base(workers: 1)
        {
            CacheDirectory = directory;
        }

        public ResourceCache(string directory) : this(new DirectoryInfo(directory))
        {
        }

        public override RequestStatus ProcessRequest(ResourceResponse request, bool prioritized)
        {
            var headers = new WebHeaderCollection();
            if (!string.IsNullOrWhiteSpace(request.Accept))
                headers.Add(HttpRequestHeader.Accept, request.Accept);

            string path = UriToCachePath(request.Uri);
            if (!File.Exists(path))
                return RequestStatus.NotFound;
            
            var resourceStream = new ResourceStream(File.OpenRead(path), headers);
            request.OnResponseStream(resourceStream);
            return request.Status;
        }

        private string UriToCachePath(Uri uri)
        {
            return Path.Combine(CacheDirectory.FullName, uri.LocalPath);
        }
    }
}
