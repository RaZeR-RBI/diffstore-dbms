using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcurrentCollections;
using Diffstore.DBMS.Core;
using Diffstore.DBMS.Core.Exceptions;
using Diffstore.Entities;
using Diffstore.Snapshots;
using Polly;

/// <summary>
/// Defines local and remote driver implementations.
/// </summary>
namespace Diffstore.DBMS.Drivers
{
    /// <summary>
    /// Defines all supported operations.
    /// </summary>
    public class EmbeddedDBMS<TKey, TValue> : IDiffstoreDBMS<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        private readonly IDiffstore<TKey, TValue> db;
        private readonly Policy<bool> lockPolicy;
        private readonly ITransactionProvider<TKey> transaction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">Existing Diffstore instance</param>
        /// <param name="policy">Transaction policy options</param>
        /// <param name="transaction">Transaction provider instance</param>
        public EmbeddedDBMS(IDiffstore<TKey, TValue> db,
            TransactionPolicyInfo policy, ITransactionProvider<TKey> transaction)
        {
            this.db = db;
            this.transaction = transaction;
            var retryPolicy = Policy
                .HandleResult<bool>(false)
                .WaitAndRetryAsync(policy.RetryTimeouts);
            var fallbackPolicy = Policy
                .HandleResult<bool>(false)
                .FallbackAsync(false, (c, r) => throw new ResourceIsBusyException());

            this.lockPolicy = fallbackPolicy.WrapAsync(retryPolicy);

            this.existence = new ConcurrentDictionary<TKey, bool>(
                db.Keys.ToDictionary(k => k, k =>
                {
                    try
                    {
                        return db.GetSnapshots(k).Any();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Entity {k} is corrupt. Deleting.", ex);
                        db.Delete(k);
                        return false;
                    }
                })
            );
        }

        private async Task AcquireForRead(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.BeginRead(key)));

        private async Task ReleaseFromRead(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.EndRead(key)));

        private async Task AcquireForWrite(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.BeginWrite(key)));

        private async Task ReleaseFromWrite(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.EndWrite(key)));

        // Entity and snapshot existence map. If a key is present, then the entity exists.
        // If the value is true, the corresponding entity has at least one snapshot.
        private ConcurrentDictionary<TKey, bool> existence = new ConcurrentDictionary<TKey, bool>();
        private IEnumerable<TKey> cachedKeys => existence.Keys;

        private async Task<T> WriteTransaction<T>(TKey key, Func<T> fn,
            bool checkEntityExistence = false)
        {
            if (checkEntityExistence && !existence.ContainsKey(key))
                throw new EntityNotFoundException(key);

            await AcquireForWrite(key);
            try
            {
                var result = fn();
                return result;
            }
            catch { throw; }
            finally { await ReleaseFromWrite(key); }
        }

        private async Task WriteTransaction(TKey key, Action action,
            bool checkEntityExistence = false)
        {
            if (checkEntityExistence && !existence.ContainsKey(key))
                throw new EntityNotFoundException(key);

            await AcquireForWrite(key);
            try
            {
                action();
            }
            catch { throw; }
            finally { await ReleaseFromWrite(key); }
        }

        private async Task<T> ReadTransaction<T>(TKey key, Func<T> func,
            bool checkSnapshotExistence = false)
        {
            if (!existence.ContainsKey(key)) throw new EntityNotFoundException(key);
            if (checkSnapshotExistence && !existence[key])
                throw new SnapshotNotFoundException(key);

            await AcquireForRead(key);
            try
            {
                return func();
            }
            catch { throw; }
            finally { await ReleaseFromRead(key); }
        }

        /// <summary>
        /// Deletes the entity and all associated data by key.
        /// </summary>
        public async Task Delete(TKey key) =>
            await WriteTransaction(key, () =>
            {
                db.Delete(key);
                existence.TryRemove(key, out _);
            });

        /// <summary>
        /// Deletes the entity and all associated data by key.
        /// </summary>
        public async Task Delete(Entity<TKey, TValue> entity) =>
            await Delete(entity.Key);

        /// <summary>
        /// Returns true if an entity with the specified key exists.
        /// </summary>
        public Task<bool> Exists(TKey key) =>
            Task.FromResult(existence.ContainsKey(key));

        /// <summary>
        /// Returns an entity saved with the specified key.
        /// </summary>
        public async Task<Entity<TKey, TValue>> Get(TKey key) =>
            await ReadTransaction(key, () => db.Get(key));

        /// <summary>
        /// Returns the first snapshot of the specified entity.
        /// </summary>
        public async Task<Snapshot<TKey, TValue>> GetFirst(TKey key) =>
            await ReadTransaction(key, () => db.GetFirst(key), checkSnapshotExistence: true);

        /// <summary>
        /// Returns the time when the first entity snapshot was created.
        /// </summary>
        public async Task<long> GetFirstTime(TKey key) =>
            await ReadTransaction(key, () => db.GetFirstTime(key), checkSnapshotExistence: true);

        /// <summary>
        /// Returns the last snapshot of the specified entity.
        /// </summary>
        public async Task<Snapshot<TKey, TValue>> GetLast(TKey key) =>
            await ReadTransaction(key, () => db.GetLast(key), checkSnapshotExistence: true);

        /// <summary>
        /// Returns the time when the last entity snapshot was created.
        /// </summary>
        public async Task<long> GetLastTime(TKey key) =>
            await ReadTransaction(key, () => db.GetLastTime(key), checkSnapshotExistence: true);

        // note: ToList() calls ensure that all data is fetched before releasing the lock
        /// <summary>
        /// Fetches snapshots for the specified entity key.
        /// </summary>
        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key) =>
            await ReadTransaction(key, () => db.GetSnapshots(key).ToList(),
                checkSnapshotExistence: true);

        /// <summary>
        /// Fetches snapshots page for the specified entity key.
        /// </summary>
        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key, int from, int count) =>
            await ReadTransaction(key, () => db.GetSnapshots(key, from, count).ToList(),
                checkSnapshotExistence: true);

        /// <summary>
        /// Fetches snapshots in time range [timeStart, timeEnd).
        /// </summary>
        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshotsBetween(TKey key, long timeStart, long timeEnd) =>
            await ReadTransaction(key, () =>
                db.GetSnapshotsBetween(key, timeStart, timeEnd).ToList(),
                checkSnapshotExistence: true);

        /// <summary>
        /// Saves an entity snapshot with the specified time.
        /// </summary>
        public async Task PutSnapshot(Entity<TKey, TValue> entity, long time) =>
            await WriteTransaction(entity.Key, () =>
            {
                var key = entity.Key;
                db.PutSnapshot(entity, time);
                existence.AddOrUpdate(key, true, (entityKey, hasSnapshots) => true);
            }, checkEntityExistence: true);

        /// <summary>
        /// Saves the entity.
        /// </summary>
        public async Task Save(Entity<TKey, TValue> entity, bool makeSnapshot = true) =>
            await WriteTransaction(entity.Key, () =>
            {
                var key = entity.Key;
                db.Save(entity, makeSnapshot);
                existence.AddOrUpdate(key,
                    makeSnapshot || db.GetSnapshots(key).Any(),
                    (entityKey, hasSnapshots) => true);
            });

        /// <summary>
        /// Saves the entity.
        /// </summary>
        public async Task Save(TKey key, TValue value, bool makeSnapshot = true) =>
            await Save(Entity.Create(key, value), makeSnapshot);

        /// <summary>
        /// Returns all saved entities.
        /// </summary>
        /// <remarks>This call may be slow depending on number of entities</remarks>
        public async Task<IEnumerable<Entity<TKey, TValue>>> GetAll() =>
            await Task.WhenAll(cachedKeys.Select(Get));

        /// <summary>
        /// Disposes this driver.
        /// </summary>
        public void Dispose()
        {
            // do nothing
        }

        /// <summary>
        /// Fetches all existing keys.
        /// </summary>
        public Task<IEnumerable<TKey>> Keys() => Task.FromResult(cachedKeys);
    }
}