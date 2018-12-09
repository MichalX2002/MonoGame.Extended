using System.Net;

namespace MonoGame.Extended.Testing
{
    public class HeaderCollection
    {
        private WebHeaderCollection _headers;

        public string this[HttpResponseHeader header] => GetString(header);

        public string ContentType => GetString(HttpResponseHeader.ContentType);
        public long ContentLength => GetInt64(HttpResponseHeader.ContentLength);

        public HeaderCollection(WebHeaderCollection headers)
        {
            _headers = headers;
        }

        public string GetString(HttpResponseHeader header)
        {
            return _headers[header];
        }

        public string GetString(string header)
        {
            return _headers[header];
        }

        public long GetInt64(HttpResponseHeader header)
        {
            if (long.TryParse(GetString(header), out long result))
                return result;
            return -1;
        }

        public int GetInt32(HttpResponseHeader header)
        {
            if (int.TryParse(GetString(header), out int result))
                return result;
            return -1;
        }
    }
}
