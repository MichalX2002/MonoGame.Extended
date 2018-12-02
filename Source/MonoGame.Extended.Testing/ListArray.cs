﻿using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

public class ListArray<T> :
    ICollection<T>, IList<T>, IReadOnlyCollection<T>,
    IReadOnlyList<T>, IEnumerable<T>, IReferenceList<T>
{
    public delegate void VersionChangedDelegate(int version);
    public event VersionChangedDelegate Changed;

    private const int _defaultCapacity = 4;
    private static readonly bool _isPrimitive;

    private int __version;
    private T[] _array;
    private int _count;

    public bool IsReadOnly { get; private set; }
    public bool IsFixedCapacity { get; private set; }
    public int Version { get => __version; protected set => SetVersion(value); }

    public T[] InnerArray => _array;
    public int Count => _count;

    public T this[int index]
    {
        get
        {
            if (index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _array[index];
        }
        set
        {
            CheckAccessibility();
            _array[index] = value;
            Version++;
        }
    }

    public int Capacity
    {
        get => _array.Length;
        set
        {
            if (IsFixedCapacity)
                throw new InvalidOperationException(
                    "This collection has a fixed capacity and cannot be resized.");

            CheckAccessibility();

            if (value != _array.Length)
            {
                if (value < _count)
                    throw new ArgumentException(
                        "The new capacity is not enough to contain existing items.", nameof(value));

                if (value > 0)
                {
                    T[] newItems = new T[value];
                    if (_count > 0)
                        Array.Copy(_array, 0, newItems, 0, _count);

                    _array = newItems;
                }
                else
                    _array = Array.Empty<T>();
                Version++;
            }
        }
    }

    static ListArray()
    {
        _isPrimitive = !typeof(T).IsValueType;
    }
    
    public ListArray()
    {
        _array = Array.Empty<T>();
    }

    public ListArray(int capacity)
    {
        _array = new T[capacity];
    }

    public ListArray(int capacity, bool fixedCapacity) : this(capacity)
    {
        IsFixedCapacity = fixedCapacity;
    }

    public ListArray(T[] sourceArray, int startOffset, int count)
    {
        _array = sourceArray;
        _count = count;
        Capacity = sourceArray.Length;
        IsFixedCapacity = true;

        if (startOffset != 0)
        {
            Array.ConstrainedCopy(sourceArray, startOffset, sourceArray, 0, count);
            Array.Clear(sourceArray, count, Capacity - count);
        }
    }

    public ListArray(T[] sourceArray, int count) : this(sourceArray, 0, count)
    {
    }

    public ListArray(T[] sourceArray) : this(sourceArray, 0, sourceArray.Length)
    {
    }

    public ListArray(IEnumerable<T> collection, bool readOnly)
    {
        if (collection is ICollection<T> c)
        {
            int count = c.Count;
            if (count == 0)
                _array = Array.Empty<T>();
            else
            {
                _array = new T[count];
                c.CopyTo(_array, 0);
                _count = count;
            }
        }
        else
        {
            _count = 0;
            _array = Array.Empty<T>();
            AddRange(collection);
        }

        IsReadOnly = readOnly;
    }

    public ListArray(IEnumerable<T> collection) : this(collection, false)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckAccessibility()
    {
        if (IsReadOnly)
            throw new InvalidOperationException("This collection is marked as read-only.");
    }

    public ref T GetReferenceAt(int index)
    {
        if (index >= _count)
            throw new IndexOutOfRangeException();

        return ref _array[index];
    }

    private void SetVersion(int value)
    {
        Changed?.Invoke(value);
        __version = value;
    }

    public void AddRef(ref T item)
    {
        AddCheck();
        _array[_count++] = item;
        Version++;
    }

    public void Add(T item)
    {
        AddCheck();
        _array[_count++] = item;
        Version++;
    }

    private void AddCheck()
    {
        CheckAccessibility();

        if (_count == _array.Length)
            EnsureCapacity(_count + 1);
    }

    public void AddRange(IEnumerable<T> collection)
    {
        InsertRange(_count, collection);
    }

    public void Sort(IComparer<T> comparer)
    {
        Sort(0, _count, comparer);
    }

    public void Sort(int index, int count, IComparer<T> comparer)
    {
        CheckAccessibility();

        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (count < 0)
            throw new ArgumentOutOfRangeException(
                nameof(count), "Needs a non-negative number.");

        if (_count - index < count)
            throw new ArgumentException("Invalid offset length.");

        Array.Sort(_array, index, count, comparer);
        Version++;
    }
    
    public void Clear()
    {
        CheckAccessibility();

        if (_count > 0)
        {
            if (_isPrimitive)
                Array.Clear(_array, 0, _count);

            _count = 0;
            Version++;
        }
    }

    public bool Contains(T item)
    {
        return IndexOf(item) != -1;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.ConstrainedCopy(_array, 0, array, arrayIndex, _count);
    }

    public bool Remove(T item)
    {
        CheckAccessibility();

        int index = IndexOf(item);
        if (index != -1)
        {
            RemoveAtInternal(index);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        GetAndRemoveAt(index);
    }

    public T GetAndRemoveAt(int index)
    {
        CheckAccessibility();

        if (index >= _count || index < 0)
            throw new IndexOutOfRangeException();
        return RemoveAtInternal(index);
    }

    public T GetAndRemoveLast()
    {
        if (_count > 0)
            return GetAndRemoveAt(_count - 1);
        return default;
    }

    public int FindIndex(Predicate<T> predicate)
    {
        for (int i = 0; i < _count; i++)
        {
            if (predicate.Invoke(_array[i]) == true)
                return i;
        }
        return -1;
    }

    private T RemoveAtInternal(int index)
    {
        _count--;
        if (index < _count)
            Array.Copy(_array, index + 1, _array, index, _count - index);

        T item = _array[_count];
        _array[_count] = default;
        Version++;
        return item;
    }

    public int IndexOf(T item)
    {
        if (item == null)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_array[i] == null)
                    return i;
            }
        }
        else
        {
            for (int i = 0; i < _count; i++)
            {
                T obj = _array[i];
                if (obj != null && obj.Equals(item))
                    return i;
            }
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        CheckAccessibility();
        InternalInsert(index, item);
    }

    private void InternalInsert(int index, T item)
    {
        if (index > Capacity)
            throw new IndexOutOfRangeException();

        if (_count == _array.Length)
            EnsureCapacity(_count + 1);

        if (index < _count)
            Array.Copy(_array, index, _array, index + 1, _count - index);

        _array[index] = item;
        _count++;
        Version++;
    }

    public void InsertRange(int index, IEnumerable<T> collection)
    {
        CheckAccessibility();

        if (collection is ICollection<T> c)
        {
            int count = c.Count;
            if (count > 0)
            {
                EnsureCapacity(_count + count);
                if (index < _count)
                    Array.Copy(_array, index, _array, index + count, count - index);

                if (c == this)
                {
                    Array.Copy(_array, 0, _array, index, index);
                    Array.Copy(_array, index + count, _array, index * 2, _count - index);
                }
                else
                    c.CopyTo(_array, index);

                _count += count;
            }
        }
        else
        {
            foreach (var item in collection)
                Insert(index++, item);
        }
    }

    public ReadOnlyCollection<T> AsReadOnly()
    {
        return new ReadOnlyCollection<T>(this);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new InvalidOperationException();
    }

    private void EnsureCapacity(int min)
    {
        if (_array.Length < min)
        {
            int oldSize = _array.Length;
            int extraSize = oldSize + oldSize / 2; // grow by x1.5
            if (extraSize > 256)
                extraSize = 256;

            int newCapacity = _array.Length + extraSize;
            if (newCapacity < _defaultCapacity)
                newCapacity = _defaultCapacity;

            Capacity = newCapacity;
        }
    }

    struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private ListArray<T> _list;
        private int _index;
        private readonly int _version;

        public T Current { get; private set; }

        object IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _list._count + 1)
                {
                    throw new InvalidOperationException(
                        "Either MoveNext has not been called or index is beyond item count.");
                }
                return Current;
            }
        }

        public Enumerator(ListArray<T> list)
        {
            _list = list;
            _index = 0;
            _version = _list.__version;
            Current = default;
        }

        public bool MoveNext()
        {
            if (_version == _list.__version && _index < _list._count)
            {
                Current = _list._array[_index];
                _index++;
                return true;
            }
            return MoveNextRare();
        }

        private bool MoveNextRare()
        {
            if (_version != _list.__version)
            {
                throw GetVersionException();
            }

            _index = _list._count + 1;
            Current = default;
            return false;
        }

        void IEnumerator.Reset()
        {
            if (_version != _list.__version)
            {
                throw GetVersionException();
            }

            _index = 0;
            Current = default;
        }

        private InvalidOperationException GetVersionException()
        {
            return new InvalidOperationException(
                "The underlying list version has changed.");
        }

        public void Dispose()
        {
        }
    }
}
