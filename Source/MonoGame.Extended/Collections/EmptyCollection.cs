using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public struct EmptyCollection<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
    {
        public static readonly EmptyCollection<T> Instance;
        public static readonly EmptyCollection<T> ReadOnlyInstance;

        public int Count => 0;
        public bool IsReadOnly { get; }
        public object SyncRoot => null;
        public bool IsSynchronized => true;

        static EmptyCollection()
        {
            Instance = new EmptyCollection<T>(false);
            ReadOnlyInstance = new EmptyCollection<T>(true);
        }

        public EmptyCollection(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
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
            return new EmptyEnumerator<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EmptyEnumerator<T>();
        }
    }
}
