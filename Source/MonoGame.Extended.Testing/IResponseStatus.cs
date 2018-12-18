using System;

namespace MonoGame.Extended.Testing
{
    public interface IResponseStatus
    {
        Uri Uri { get; }
        long ContentLength { get; }
        long BytesDownloaded { get; }
        
        bool IsNotFound { get; }
        bool IsComplete { get; }
        bool IsCanceled { get; }
        bool IsFaulted { get; }
        Exception Fault { get; }

        void Cancel();
    }
}
