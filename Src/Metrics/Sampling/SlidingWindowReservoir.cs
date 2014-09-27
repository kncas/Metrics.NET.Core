﻿using System;
using System.Threading;
using Metrics.Utils;

namespace Metrics.Sampling
{
    public sealed class SlidingWindowReservoir : Reservoir
    {
        private const int DefaultSize = 1028;

        private readonly long[] values;
        private AtomicLong count = new AtomicLong();

        public SlidingWindowReservoir()
            : this(DefaultSize) { }

        public SlidingWindowReservoir(int size)
        {
            this.values = new long[size];
        }

        public void Update(long value, string userValue = null)
        {
            var count = this.count.Increment();
            this.values[(int)((count - 1) % values.Length)] = value;
        }

        public void Reset()
        {
            Array.Clear(this.values, 0, values.Length);
            count.SetValue(0L);
        }

        public int Size { get { return Math.Min((int)this.count.Value, values.Length); } }

        public Snapshot Snapshot
        {
            get
            {
                var size = this.Size;
                long[] values = new long[size];

                for (int i = 0; i < size; i++)
                {
                    values[i] = Interlocked.Read(ref this.values[i]);
                }
                return new UniformSnapshot(values);
            }
        }
    }
}