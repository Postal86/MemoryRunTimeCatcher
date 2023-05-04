using System;
using System.Collections.Concurrent;

namespace MemoryOffsetLocator.Engine.Common.DataStructures
{
    public class TtlCache<V>
    {
        protected static readonly Random Random = new Random();


        private ConcurrentDictionary<V, DateTime> cache;

        public TtlCache()
        {
            this.cache = new ConcurrentDictionary<V, DateTime>();
            this.DefaultTimeToLive = TimeSpan.MaxValue;
        }

        public TtlCache(TimeSpan  defaultTimeToLive) : this()
        {
            this.DefaultTimeToLive = defaultTimeToLive;
        }

        public TtlCache(TimeSpan  defaultTimeToLive, TimeSpan randomTimeToLiveOffset) : this(defaultTimeToLive)
        {
            this.RandomTimeToLiveOffset = randomTimeToLiveOffset;
        }

        protected TimeSpan DefaultTimeToLive { get; set; }

        protected TimeSpan RandomTimeToLiveOffset { get; set; }

        public void Invalidate()
        {
            this.cache.Clear();
        }

        public virtual void Add(V  value)
        {
             if (value == null)
            {
                return;
            }


             if (this.RandomTimeToLiveOffset != null)
            {
                Int32 maximumOffset = (Int32)this.RandomTimeToLiveOffset.TotalMilliseconds;
                TimeSpan offset = TimeSpan.FromMilliseconds(TtlCache<V>.Random.Next(-maximumOffset, maximumOffset));
                TimeSpan timeToLive = this.DefaultTimeToLive + offset;

                this.Add(value, timeToLive);
            }
             else
            {
                this.Add(value, this.DefaultTimeToLive);
            }
        }

        public virtual void Add(V value, TimeSpan timeToLive)
        {
            if (value == null)
            {
                return;
            }

            DateTime expireTime = timeToLive == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.Now + timeToLive;

            this.cache.AddOrUpdate(value, expireTime, (key, ttl) => { return ttl; });
        }

        public  Boolean  Contains(V value)
        {

        }
    }
}
