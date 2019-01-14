using System.IO;
using System.Net;

namespace MonoGame.Extended.Testing
{
    public partial class ResourceRequest
    {
        public void HandleOnResponse(string file, WebHeaderCollection headers)
        {
            if (!AssertNotCanceled())
                return;

            if (!File.Exists(file))
            {
                SetNotFound(WebExceptionStatus.CacheEntryNotFound);
                return;
            }

            var resourceStream = new ResourceStream(File.OpenRead(file), headers);
            HandleOnResponse(resourceStream);
        }
    }
}
