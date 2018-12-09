using System;

namespace MonoGame.Extended.Testing
{
    public interface IResponseStatus
    {
        Uri Url { get; }
        long ContentLength { get; }
        long BytesDownloaded { get; }
        
        bool IsComplete { get; }
        bool IsCanceled { get; }
        bool IsFaulted { get; }
        Exception Fault { get; }
    }
}
