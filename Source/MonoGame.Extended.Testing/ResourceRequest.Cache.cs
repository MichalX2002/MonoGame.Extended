using System.IO;
using System.Net;

namespace MonoGame.Extended.Testing
{
    internal partial class ResourceRequest
    {
        public void HandleOnResponse(FileInfo file, WebHeaderCollection headers)
        {
            if (IsCanceled)
            {
                Fault = new WebException(EXCEPTION_CANCELED, WebExceptionStatus.RequestCanceled);
                return;
            }

            if (!file.Exists)
            {
                Fault = new WebException("The resource was not cached.", WebExceptionStatus.CacheEntryNotFound);
                SetNotFound();
                return;
            }

            var resourceStream = new ResourceStream(file.OpenRead(), headers);
            HandleOnResponse(resourceStream);
        }
    }
}
