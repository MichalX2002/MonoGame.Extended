using System.Collections.Generic;

namespace MonoGame.Extended
{ 
    public interface IReferenceList<T> : IList<T>
    {
        ref T GetReferenceAt(int index);
    }
}
