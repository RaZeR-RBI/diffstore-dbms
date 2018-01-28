using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diffstore.DBMS.Core.Exceptions;
using Diffstore.Entities;
using Diffstore.Snapshots;
using Polly;

namespace Diffstore.DBMS.Core
{
    public class PollyAsyncBackend<TKey, TValue> : IAsyncBackend<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        private readonly IDiffstore<TKey, TValue> db;
        private readonly Policy<bool> lockPolicy;
        private readonly ITransactionProvider<TKey> transaction;

        public PollyAsyncBackend(IDiffstore<TKey, TValue> db,
            TransactionPolicyInfo policy, ITransactionProvider<TKey> transaction)
        {
            this.db = db;
            this.transaction = transaction;
            this.lockPolicy = Policy
                .HandleResult<bool>(acquired => !acquired)
                .WaitAndRetry(policy.RetryTimeouts);
        }

        private async Task AcquireForRead(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.BeginRead(key)));

        private async Task ReleaseFromRead(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.EndRead(key)));

        private async Task AcquireForWrite(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.BeginWrite(key)));

        private async Task ReleaseFromWrite(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.EndWrite(key)));

        public Task<IEnumerable<TKey>> Keys => throw new NotImplementedException();

        public Task<IEnumerable<Entity<TKey, TValue>>> Entities => throw new NotImplementedException();

        private async Task<T> WriteTransaction<T>(TKey key, Func<T> fn)
        {
            await AcquireForWrite(key);
            try
            {
                var result = fn();
                return result;
            }
            catch { throw; }
            finally { await ReleaseFromWrite(key); }
        }

        private async Task WriteTransaction(TKey key, Action action)
        {
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
            await AcquireForRead(key);
            try
            {
                if (!await Exists(key)) throw new EntityNotFoundException(key);
                if (checkSnapshotExistence && !(await GetSnapshots(key)).Any())
                    throw new SnapshotNotFoundException(key);
                return func();
            }
            catch { throw; }
            finally { await ReleaseFromRead(key); }
        }

        public async Task Delete(TKey key) => 
            await WriteTransaction(key, () => db.Delete(key));

        public async Task Delete(Entity<TKey, TValue> entity) => 
            await Delete(entity.Key);

        public async Task<bool> Exists(TKey key) => 
            await WriteTransaction(key, () => db.Exists(key));

        public async Task<Entity<TKey, TValue>> Get(TKey key) =>
            await ReadTransaction(key, () => db.Get(key));

        public async Task<Snapshot<TKey, TValue>> GetFirst(TKey key) =>
            await ReadTransaction(key, () => db.GetFirst(key), checkSnapshotExistence: true);

        public async Task<long> GetFirstTime(TKey key) =>
            await ReadTransaction(key, () => db.GetFirstTime(key), checkSnapshotExistence: true);

        public async Task<Snapshot<TKey, TValue>> GetLast(TKey key) =>
            await ReadTransaction(key, () => db.GetLast(key), checkSnapshotExistence: true);

        public async Task<long> GetLastTime(TKey key) =>
            await ReadTransaction(key, () => db.GetLastTime(key), checkSnapshotExistence: true);

        // note: ToList() calls ensure that all data is fetched before releasing the lock
        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key) =>
            await ReadTransaction(key, () => db.GetSnapshots(key).ToList(), checkSnapshotExistence: true);

        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key, int from, int count) =>
            await ReadTransaction(key, () => db.GetSnapshots(key, from, count).ToList(), 
                checkSnapshotExistence: true);

        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key, long timeStart, long timeEnd) =>
            await ReadTransaction(key, () => db.GetSnapshotsBetween(key, timeStart, timeEnd).ToList(),
                checkSnapshotExistence: true);

        public async Task PutSnapshot(Entity<TKey, TValue> entity, long time) =>
            await WriteTransaction(entity.Key, () => db.PutSnapshot(entity, time));

        public async Task Save(Entity<TKey, TValue> entity, bool makeSnapshot = true) =>
            await WriteTransaction(entity.Key, () => db.Save(entity, makeSnapshot));

        public async Task Save(TKey key, TValue value, bool makeSnapshot = true) =>
            await Save(Entity.Create(key, value), makeSnapshot);
    }
}