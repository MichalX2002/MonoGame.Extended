using System.Net;

namespace MonoGame.Extended.Testing
{
    public class ResourceDownloader : ResourceRequester
    {
        public ResourceDownloader() : base(workers: 2)
        {
        }

        public override void OnRequest(ResourceRequest resourceRequest)
        {
            var request = WebRequest.CreateHttp(resourceRequest.Uri);
            request.Accept = resourceRequest.Accept;

            using (var response = request.GetResponse())
                resourceRequest.HandleOnResponse(response as HttpWebResponse);
        }
    }
}