using System;
using System.Net;

namespace MonoGame.Extended.Testing
{
    public class ResourceDownloader : ResourceRequester
    {
        public ResourceDownloader() : base(workers: 2)
        {
        }

        public override RequestStatus ProcessRequest(ResourceResponse request, bool prioritized)
        {
            try
            {
                var httpRequest = WebRequest.CreateHttp(request.Uri);
                httpRequest.Accept = request.Accept;

                using (var httpResponse = httpRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                        return RequestStatus.NotFound;

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                        return RequestStatus.ProtocolError;
                    
                    var resourceStream = new ResourceStream(httpResponse);
                    request.OnResponseStream(resourceStream);
                    return request.Status;
                }
            }
            catch (Exception exc)
            {
                request.Fault = exc;
                return RequestStatus.Faulted;
            }
        }
    }
}