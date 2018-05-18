using System.Collections.Generic;

namespace MonoGame.Extended
{
    public interface IReferencedEnumerable<T>
    {
        ref IEnumerator<T> GetEnumeratorReference();
    }
}
