using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConcurrentCollections;

namespace Diffstore.DBMS.Core
{
    public class ConcurrentTransactionProvider<TKey> : ITransactionProvider<TKey>
        where TKey : IComparable
    {
        private readonly ConcurrentHashSet<TKey> markers = new ConcurrentHashSet<TKey>();
        public IEnumerable<TKey> InProgress => markers;

        public bool Begin(TKey key) => markers.Add(key);

        public bool End(TKey key) => markers.TryRemove(key);

        public bool IsAvailable(TKey key) => markers.Contains(key);
    }
}