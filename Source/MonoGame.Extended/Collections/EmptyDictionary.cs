using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public class EmptyDictionary<TKey, TValue> : 
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        IReadOnlyDictionary<TKey, TValue>,
        IDictionary<TKey, TValue>, IDictionary,
        IEnumerable, ICollection
    {
        public static readonly EmptyDictionary<TKey, TValue> Instance;

        public ICollection<TKey> Keys { get; }
        public ICollection<TValue> Values { get; }

        ICollection IDictionary.Keys => (ICollection)Keys;
        ICollection IDictionary.Values => (ICollection)Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public int Count => 0;
        public bool IsReadOnly => true;
        public bool IsFixedSize => true;
        public object SyncRoot => null;
        public bool IsSynchronized => true;
        
        public TValue this[TKey key] { get => default; set { } }
        public object this[object key] { get => null; set { } }

        static EmptyDictionary()
        {
            Instance = new EmptyDictionary<TKey, TValue>();
        }

        public EmptyDictionary()
        {
            Keys = EmptyCollection<TKey>.Instance;
            Values = EmptyCollection<TValue>.Instance;
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
            return EmptyEnumerator<KeyValuePair<TKey, TValue>>.Instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EmptyEnumerator<KeyValuePair<TKey, TValue>>.Instance;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return EmptyDictionaryEnumerator.Instance;
        }
    }
}
