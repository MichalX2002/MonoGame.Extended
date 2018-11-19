using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{ 
    public interface IReferenceList<T> : IList<T>
    {
        ref T GetReferenceAt(int index);
        void AddRef(ref T item);
    }
}
