using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public class EmptyCollection<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
    {
        public static readonly EmptyCollection<T> Instance;

        public int Count => 0;
        public bool IsReadOnly => true;
        public object SyncRoot => null;
        public bool IsSynchronized => true;

        static EmptyCollection()
        {
            Instance = new EmptyCollection<T>();
        }

        public void Add(T item)
        {
        }

        public void Clear()
        {
        }

        public bool Contains(T item)
        {
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
        }

        public bool Remove(T item)
        {
            return false;
        }

        public void CopyTo(Array array, int index)
        {
        }

        public IEnumerator<T> GetEnumerator()
        {
            return EmptyEnumerator<T>.Instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EmptyEnumerator<T>.Instance;
        }
    }
}
