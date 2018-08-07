using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public struct EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current => default;
        object IEnumerator.Current => null;

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public void Dispose()
        {
        }
    }
}
