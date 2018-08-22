using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public class EmptyEnumerator<T> : IEnumerator<T>
    {
        public static readonly EmptyEnumerator<T> Instance;

        public T Current => default;
        object IEnumerator.Current => null;

        static EmptyEnumerator()
        {
            Instance = new EmptyEnumerator<T>();
        }

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
