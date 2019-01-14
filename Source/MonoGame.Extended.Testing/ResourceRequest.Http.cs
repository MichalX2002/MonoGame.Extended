using System.Net;

namespace MonoGame.Extended.Testing
{
    public partial class ResourceRequest
    {
        public void HandleOnResponse(HttpWebResponse response)
        {
            if (!AssertNotCanceled())
                return;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                SetNotFound(WebExceptionStatus.ProtocolError);
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                FaultStatus = WebExceptionStatus.ProtocolError;
                return;
            }

            var resourceStream = new ResourceStream(response);
            HandleOnResponse(resourceStream);
        }
    }
}
