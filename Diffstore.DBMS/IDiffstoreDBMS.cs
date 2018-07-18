using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Diffstore.DBMS.Core;
using Diffstore.DBMS.Drivers;
using Diffstore.Entities;
using Diffstore.Snapshots;

/// <summary>
/// Main namespace which contains the storage interface
/// </summary>
namespace Diffstore.DBMS
{
    /// <summary>
    /// Defines all supported operations.
    /// </summary>
    public interface IDiffstoreDBMS<TKey, TValue> : IDisposable
        where TKey : IComparable
        where TValue : class, new()
    {
        /// <summary>
        /// Fetches all existing keys.
        /// </summary>
        Task<IEnumerable<TKey>> Keys();
        /// <summary>
        /// Deletes the entity and all associated data by key.
        /// </summary>
        Task Delete(TKey key);
        /// <summary>
        /// Deletes the entity and all associated data by key.
        /// </summary>
        Task Delete(Entity<TKey, TValue> entity);
        /// <summary>
        /// Returns true if an entity with the specified key exists.
        /// </summary>
        Task<bool> Exists(TKey key);
        /// <summary>
        /// Returns an entity saved with the specified key.
        /// </summary>
        Task<Entity<TKey, TValue>> Get(TKey key);
        /// <summary>
        /// Returns all saved entities.
        /// </summary>
        /// <remarks>This call may be slow depending on number of entities</remarks>
        Task<IEnumerable<Entity<TKey, TValue>>> GetAll();
        /// <summary>
        /// Returns the first snapshot of the specified entity.
        /// </summary>
        Task<Snapshot<TKey, TValue>> GetFirst(TKey key);
        /// <summary>
        /// Returns the time when the first entity snapshot was created.
        /// </summary>
        Task<long> GetFirstTime(TKey key);
        /// <summary>
        /// Returns the last snapshot of the specified entity.
        /// </summary>
        Task<Snapshot<TKey, TValue>> GetLast(TKey key);
        /// <summary>
        /// Returns the time when the last entity snapshot was created.
        /// </summary>
        Task<long> GetLastTime(TKey key);
        /// <summary>
        /// Fetches snapshots for the specified entity key.
        /// </summary>
        Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key);
        /// <summary>
        /// Fetches snapshots page for the specified entity key.
        /// </summary>
        Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key, int from, int count);
        /// <summary>
        /// Fetches snapshots in time range [timeStart, timeEnd).
        /// </summary>
        Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshotsBetween(TKey key, long timeStart, long timeEnd);
        /// <summary>
        /// Saves an entity snapshot with the specified time.
        /// </summary>
        Task PutSnapshot(Entity<TKey, TValue> entity, long time);
        /// <summary>
        /// Saves the entity.
        /// </summary>
        Task Save(Entity<TKey, TValue> entity, bool makeSnapshot = true);
        /// <summary>
        /// Saves the entity.
        /// </summary>
        Task Save(TKey key, TValue value, bool makeSnapshot = true);
    }

    /// <summary>
    /// Main class for DBMS driver initialization.
    /// </summary>
    public static class DiffstoreDBMS
    {
        /// <summary>
        /// Creates an embedded (local storage) Diffstore DBMS instance.
        /// </summary>
        /// <param name="db">Existing Diffstore instance</param>
        /// <param name="provider">ITransactionProvider</param>
        /// <param name="policy">Transaction policy options</param>
        public static IDiffstoreDBMS<TKey, TValue> Embedded<TKey, TValue>(
            IDiffstore<TKey, TValue> db,
            ITransactionProvider<TKey> provider, TransactionPolicyInfo policy)
            where TKey : IComparable
            where TValue : class, new() =>
                new EmbeddedDBMS<TKey, TValue>(db, policy, provider);

        /// <summary>
        /// Initializes a remote DBMS driver.
        /// </summary>
        public static IDiffstoreDBMS<TKey, TValue> Remote<TKey, TValue>(
            Uri connectionUri)
        where TKey : IComparable
        where TValue : class, new() =>
            new RemoteDBMS<TKey, TValue>(connectionUri);

        /// <summary>
        /// Initializes a remote DBMS driver with the default connection URI.
        /// </summary>
        public static IDiffstoreDBMS<TKey, TValue> Remote<TKey, TValue>()
        where TKey : IComparable
        where TValue : class, new() =>
            new RemoteDBMS<TKey, TValue>(new Uri("http://localhost:8008/"));
    }
}
