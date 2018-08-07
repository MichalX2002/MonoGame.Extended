using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public struct EmptyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        public static EmptyDictionary<TKey, TValue> Instance;
        public static EmptyDictionary<TKey, TValue> ReadOnlyInstance;

        public ICollection<TKey> Keys => EmptyCollection<TKey>.ReadOnlyInstance;
        public ICollection<TValue> Values => EmptyCollection<TValue>.ReadOnlyInstance;

        ICollection IDictionary.Keys => EmptyCollection<TKey>.ReadOnlyInstance;
        ICollection IDictionary.Values => EmptyCollection<TValue>.ReadOnlyInstance;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => EmptyCollection<TKey>.ReadOnlyInstance;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => EmptyCollection<TValue>.ReadOnlyInstance;

        public int Count => 0;
        public bool IsReadOnly { get; }
        public bool IsFixedSize => true;
        public object SyncRoot => null;
        public bool IsSynchronized => true;
        
        public TValue this[TKey key] { get => default; set { } }
        public object this[object key] { get => null; set { } }

        static EmptyDictionary()
        {
            Instance = new EmptyDictionary<TKey, TValue>(false);
            ReadOnlyInstance = new EmptyDictionary<TKey, TValue>(true);
        }

        public EmptyDictionary(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }

        public void Add(TKey key, TValue value)
        {
        }

        public void Add(object key, object value)
        {
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
        }

        public void Clear()
        {
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public bool Contains(object key)
        {
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default;
            return false;
        }

        public bool Remove(TKey key)
        {
            return false;
        }

        public void Remove(object key)
        {
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        }

        public void CopyTo(Array array, int index)
        {
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new EmptyEnumerator<KeyValuePair<TKey, TValue>>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EmptyEnumerator<KeyValuePair<TKey, TValue>>();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new EmptyDictionaryEnumerator();
        }
    }
}
