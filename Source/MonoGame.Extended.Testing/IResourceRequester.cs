using System;

namespace MonoGame.Extended.Testing
{
    public delegate void OnErrorDelegate(Uri uri, Exception exception);
    public delegate void OnResponseDelegate(Uri uri, ResourceStream stream);

    public interface IResourceRequester : IDisposable
    {
        bool IsDisposed { get; }
        bool IsRunning { get; }

        IResponseStatus Request(string uri, string accept, OnResponseDelegate onResponse, OnErrorDelegate onError);
        IResponseStatus Request(Uri uri, string accept, OnResponseDelegate onResponse, OnErrorDelegate onError);
    }
}
