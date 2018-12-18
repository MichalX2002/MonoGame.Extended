using System.Net;

namespace MonoGame.Extended.Testing
{
    internal partial class ResourceRequest
    {
        public void HandleOnResponse(HttpWebResponse response)
        {
            if (IsCanceled)
            {
                Fault = new WebException(EXCEPTION_CANCELED, WebExceptionStatus.RequestCanceled);
                return;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                string errorMsg = "The resource was not found: " + response.StatusDescription;
                Fault = new WebException(errorMsg, WebExceptionStatus.ReceiveFailure);
                SetNotFound();
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Fault = new WebException("The response was invalid.", WebExceptionStatus.ProtocolError);
                return;
            }

            var resourceStream = new ResourceStream(response);
            HandleOnResponse(resourceStream);
        }
    }
}
