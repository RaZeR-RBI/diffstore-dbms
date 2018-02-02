using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConcurrentCollections;

namespace Diffstore.DBMS.Core
{
    public class ConcurrentTransactionProvider<TKey> : ITransactionProvider<TKey>
        where TKey : IComparable
    {
        private readonly ConcurrentHashSet<TKey> readLocks = new ConcurrentHashSet<TKey>();
        private readonly ConcurrentHashSet<TKey> writeLocks = new ConcurrentHashSet<TKey>();

        public IEnumerable<TKey> InRead => readLocks; 

        public IEnumerable<TKey> InWrite => writeLocks;

        public bool BeginRead(TKey key){
            if (writeLocks.Contains(key)) return false;
            readLocks.Add(key);
            return true;
        }

        public bool BeginWrite(TKey key)
        {
            if (readLocks.Contains(key)) return false;
            writeLocks.Add(key);
            return true;
        }

        public bool EndRead(TKey key) => readLocks.TryRemove(key) || true;

        public bool EndWrite(TKey key) => writeLocks.TryRemove(key) || true;
    }
}