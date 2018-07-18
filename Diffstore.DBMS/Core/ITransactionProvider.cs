using System;
using System.Collections.Generic;

namespace Diffstore.DBMS.Core
{
    /// <summary>
    /// Defines a transactional provider for reading and writing purposes.
    /// </summary>
    public interface ITransactionProvider<TKey>
        where TKey : IComparable
    {
        /// <summary>
        /// Locks the specified entity key for reading.
        /// </summary>
        /// <returns>True if locking was successful</returns>
        bool BeginRead(TKey key);
        /// <summary>
        /// Removes read lock from the specified entity key.
        /// </summary>
        /// <returns>True if unlocking was successful</returns>
        bool EndRead(TKey key);
        /// <summary>
        /// Locks the specified entity key for writing.
        /// </summary>
        /// <returns>True if locking was successful</returns>
        bool BeginWrite(TKey key);
        /// <summary>
        /// Removes write lock from the specified entity key.
        /// </summary>
        /// <returns>True if unlocking was successful</returns>
        bool EndWrite(TKey key);
        /// <summary>
        /// Returns a list of keys which have been locked for reading.
        /// </summary>
        IEnumerable<TKey> InRead { get; }
        /// <summary>
        /// Returns a list of keys which have been locked for writing.
        /// </summary>
        IEnumerable<TKey> InWrite { get; }
    }

    /// <summary>
    /// Helper class for <see cref="TransactionProvider" /> creation.
    /// </summary>
    public static class TransactionProvider
    {
        /// <summary>
        /// Creates a default transaction provider for the specified key type.
        /// </summary>
        public static ITransactionProvider<TKey> OfType<TKey>() where TKey : IComparable =>
            new ConcurrentTransactionProvider<TKey>();
    }
}