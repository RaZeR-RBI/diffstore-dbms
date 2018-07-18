using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConcurrentCollections;

namespace Diffstore.DBMS.Core
{
    /// <summary>
    /// Defines a concurrent transactional provider for reading and writing purposes.
    /// </summary>
    public class ConcurrentTransactionProvider<TKey> : ITransactionProvider<TKey>
        where TKey : IComparable
    {
        private readonly ConcurrentHashSet<TKey> readLocks = new ConcurrentHashSet<TKey>();
        private readonly ConcurrentHashSet<TKey> writeLocks = new ConcurrentHashSet<TKey>();

        /// <summary>
        /// Returns a list of keys which have been locked for reading.
        /// </summary>
        public IEnumerable<TKey> InRead => readLocks; 

        /// <summary>
        /// Returns a list of keys which have been locked for writing.
        /// </summary>
        public IEnumerable<TKey> InWrite => writeLocks;

        /// <summary>
        /// Locks the specified entity key for reading.
        /// </summary>
        /// <returns>True if locking was successful</returns>
        public bool BeginRead(TKey key){
            if (writeLocks.Contains(key)) return false;
            readLocks.Add(key);
            return true;
        }

        /// <summary>
        /// Locks the specified entity key for writing.
        /// </summary>
        /// <returns>True if locking was successful</returns>
        public bool BeginWrite(TKey key)
        {
            if (readLocks.Contains(key) || writeLocks.Contains(key)) return false;
            writeLocks.Add(key);
            return true;
        }

        /// <summary>
        /// Removes read lock from the specified entity key.
        /// </summary>
        /// <returns>True if unlocking was successful</returns>
        public bool EndRead(TKey key) => readLocks.TryRemove(key) | true;

        /// <summary>
        /// Removes write lock from the specified entity key.
        /// </summary>
        /// <returns>True if unlocking was successful</returns>
        public bool EndWrite(TKey key) => writeLocks.TryRemove(key) | true;
    }
}