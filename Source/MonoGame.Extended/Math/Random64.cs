using System;

namespace MonoGame.Extended
{
    public class Random64
    {
        private const long MAX = Int64.MaxValue;
        private const double DIVIDER = 1.0 / Int64.MaxValue;
        private const long MSEED = 161803398;
        
        private long _inext;
        private long _inextp;
        private long[] _seedArray = new long[56];

        public Random64() : this(Environment.TickCount) { }

        public Random64(long seed)
        {
            long ii, mj;
            long mk = 1;

            long subtraction = (seed == Int64.MinValue) ? Int64.MaxValue : Math.Abs(seed);
            mj = MSEED - subtraction;
            _seedArray[55] = mj;
            for (int i = 1; i < 55; i++)
            {
                ii = (21 * i) % 55;
                _seedArray[ii] = mk;
                mk = mj - mk;
                if (mk < 0) mk += MAX;
                mj = _seedArray[ii];
            }
            for (int k = 1; k < 5; k++)
            {
                for (int i = 1; i < 56; i++)
                {
                    _seedArray[i] -= _seedArray[1 + (i + 30) % 55];
                    if (_seedArray[i] < 0) _seedArray[i] += MAX;
                }
            }
            _inext = 0;
            _inextp = 21;
            seed = 1;
        }
        
        public virtual double NextDouble()
        {
            //Including this division gives improved random number distribution.
            return InternalSample() * DIVIDER;
        }

        private long InternalSample()
        {
            long retVal;
            long locINext = _inext;
            long locINextp = _inextp;

            if (++locINext >= 56) locINext = 1;
            if (++locINextp >= 56) locINextp = 1;

            retVal = _seedArray[locINext] - _seedArray[locINextp];

            if (retVal == MAX) retVal--;
            if (retVal < 0) retVal += MAX;
            
            _inext = locINext;
            _inextp = locINextp;

            return _seedArray[locINext] = retVal;
        }
        
        public virtual long Next()
        {
            return InternalSample();
        }
        
        public virtual long Next(long minValue, long maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));

            long range = maxValue - minValue;
            return ((Next() * range) + minValue);
        }
        
        public virtual long Next(long maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue));

            return (long)(NextDouble() * maxValue);
        }

        public virtual void NextBytes(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)((InternalSample() * DIVIDER) * (Byte.MaxValue + 1));
            }
        }
    }
}
