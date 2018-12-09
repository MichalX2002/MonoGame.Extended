using System;
using System.IO;
using System.Net;

namespace MonoGame.Extended.Testing
{
    public class ResourceStream : Stream
    {
        private WebResponse _response;
        private Stream _stream;
        private long _position;
        private long _length;

        public override bool CanWrite => false;
        public override bool CanSeek => true;
        public override bool CanRead => _stream.CanRead;
        public override long Position { get => _position; set => Seek(value, SeekOrigin.Begin); }
        
        public HeaderCollection Headers { get; }
        public string ContentType => Headers.ContentType;
        public override long Length => _length;

        internal ResourceStream(Stream stream, WebHeaderCollection headers)
        {
            _stream = stream;
            Headers = new HeaderCollection(headers);
            _length = Headers.ContentLength;
        }

        internal ResourceStream(WebResponse response) :
            this(response.GetResponseStream(), response.Headers)
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = _stream.Read(buffer, offset, count);
            _position += read;
            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}