using System;

namespace MonoGame.Extended.Testing
{
    public class ResourceCache : IResourceRequester
    {
        public bool IsDisposed => throw new NotImplementedException();
        public bool IsRunning => throw new NotImplementedException();

        public IResponseStatus Request(string uri, string accept, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            throw new NotImplementedException();
        }

        public IResponseStatus Request(Uri uri, string accept, OnResponseDelegate onResponse, OnErrorDelegate onError)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
