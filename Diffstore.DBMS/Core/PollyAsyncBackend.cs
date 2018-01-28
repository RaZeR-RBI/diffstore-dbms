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

        private async Task AcquireLock(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.Begin(key)));

        private async Task ReleaseLock(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.End(key)));

        private async Task Availability(TKey key) =>
            await lockPolicy.ExecuteAsync(() => Task.FromResult(transaction.IsAvailable(key)));

        public Task<Entity<TKey, TValue>> this[TKey key] => Get(key);

        public Task<IEnumerable<TKey>> Keys => throw new NotImplementedException();

        public Task<IEnumerable<Entity<TKey, TValue>>> Entities => throw new NotImplementedException();

        private async Task<T> Transaction<T>(TKey key, Func<T> fn)
        {
            await AcquireLock(key);
            try
            {
                var result = fn();
                return result;
            }
            catch { throw; }
            finally { await ReleaseLock(key); }
        }

        private async Task Transaction(TKey key, Action action)
        {
            await AcquireLock(key);
            try
            {
                action();
            }
            catch { throw; }
            finally { await ReleaseLock(key); }
        }

        public async Task Delete(TKey key) => await Transaction(key, () => db.Delete(key));

        public async Task Delete(Entity<TKey, TValue> entity) => await Delete(entity.Key);

        public async Task<bool> Exists(TKey key) => await Transaction(key, () => db.Exists(key));

        public async Task<Entity<TKey, TValue>> Get(TKey key)
        {
            await Availability(key);
            if (!await Exists(key)) throw new EntityNotFoundException(key);
            return db[key];
        }

        public Task<Snapshot<TKey, TValue>> GetFirst(TKey key)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetFirstTime(TKey key)
        {
            throw new NotImplementedException();
        }

        public Task<Snapshot<TKey, TValue>> GetLast(TKey key)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetLastTime(TKey key)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key, int from, int count)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key, long timeStart, long timeEnd)
        {
            throw new NotImplementedException();
        }

        public Task PutSnapshot(Entity<TKey, TValue> entity, long time)
        {
            throw new NotImplementedException();
        }

        public Task Save(Entity<TKey, TValue> entity, bool makeSnapshot = true)
        {
            throw new NotImplementedException();
        }

        public Task Save(TKey key, TValue value, bool makeSnapshot = true) =>
            Save(Entity.Create(key, value), makeSnapshot);
    }
}