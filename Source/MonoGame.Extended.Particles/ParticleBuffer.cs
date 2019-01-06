using System;
using System.Runtime.InteropServices;

namespace MonoGame.Extended.Particles
{
    public class ParticleBuffer : IDisposable
    {
        private readonly IntPtr _nativePointer;

        // points to the first memory pos after the buffer
        protected readonly unsafe Particle* BufferEnd;

        // points to the particle after the last active particle.
        protected unsafe Particle* Tail;

        // pointer to the first particle
        protected unsafe Particle* Head;

        public bool IsDisposed { get; private set; }
        public int Size { get; }
        
        // Number of available particle spots in the buffer
        public int Available => Size - Count;

        // current number of particles
        public int Count { get; private set; }

        // total size of the buffer
        public int SizeInBytes => Particle.SizeInBytes * (Size + 1);

        // total size of active particles
        public int ActiveSizeInBytes => Particle.SizeInBytes * Count;

        public unsafe ParticleBuffer(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));
            Size = size;

            // add one extra spot in memory for margin between head and tail
            // so the iterator can see that it's at the end
            _nativePointer = Marshal.AllocHGlobal(SizeInBytes);
            
            BufferEnd = (Particle*)(_nativePointer + SizeInBytes);
            Clear(); // clear to initialize head and tail
            
            GC.AddMemoryPressure(SizeInBytes);
        }

        public unsafe void Clear()
        {
            Count = 0;
            Head = (Particle*)_nativePointer;
            Tail = (Particle*)_nativePointer;
        }

        public Iterator GetIterator()
        {
            return new Iterator(this);
        }

        /// <summary>
        /// Release the given number of particles or the most available.
        /// Returns an iterator with offset to iterate over the new particles.
        /// </summary>
        public unsafe Iterator Release(int releaseQuantity)
        {
            int numToRelease = Math.Min(releaseQuantity, Available);

            int prevCount = Count;
            Count += numToRelease;

            Tail += numToRelease;
            if (Tail >= BufferEnd)
                Tail -= Size + 1;
            
            return new Iterator(this, prevCount);
        }

        public unsafe void Reclaim(int number)
        {
            Count -= number;

            Head += number;
            if (Head >= BufferEnd)
                Head -= Size + 1;
        }

        //public void CopyTo(IntPtr destination)
        //{
        //    memcpy(destination, _nativePointer, ActiveSizeInBytes);
        //}

        //public void CopyToReverse(IntPtr destination)
        //{
        //    var offset = 0;
        //    for (var i = ActiveSizeInBytes - Particle.SizeInBytes; i >= 0; i -= Particle.SizeInBytes)
        //    {
        //        memcpy(IntPtr.Add(destination, offset), IntPtr.Add(_nativePointer, i), Particle.SizeInBytes);
        //        offset += Particle.SizeInBytes;
        //    }
        //}

        //[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        //public static extern void memcpy(IntPtr dest, IntPtr src, int count);
        
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Marshal.FreeHGlobal(_nativePointer);
                IsDisposed = true;

                GC.RemoveMemoryPressure(SizeInBytes);
            }

            GC.SuppressFinalize(this);
        }

        ~ParticleBuffer()
        {
            Dispose();
        }

        public struct Iterator
        {
            private readonly ParticleBuffer _buffer;
            private unsafe Particle* _current;

            public int Total { get; private set; }
            public unsafe bool HasNext => _current != _buffer.Tail;

            public unsafe Iterator(ParticleBuffer buffer, int offset)
            {
                _buffer = buffer;
                _current = _buffer.Head;
                Total = _buffer.Count;

                if (offset != 0)
                {
                    _current = _buffer.Head + offset;
                    if (_current >= _buffer.BufferEnd)
                        _current -= _buffer.Size + 1;
                }
            }

            public unsafe Iterator(ParticleBuffer buffer) : this(buffer, 0)
            {
            }
            
            public unsafe Particle* Next()
            {
                Particle* p = _current;
                _current++;
                if (_current == _buffer.BufferEnd)
                    _current = (Particle*)_buffer._nativePointer;
                return p;
            }

            public unsafe void Reset()
            {
                _current = _buffer.Head;
                Total = _buffer.Count;
            }
        }
    }
}