using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diffstore.Entities;
using Diffstore.Snapshots;
using Polly;

namespace Diffstore.DBMS.Core
{
    public class PollyAsyncBackend<TKey, TValue> : IAsyncBackend<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        private readonly Policy<bool> lockPolicy;
        private readonly ITransactionProvider<TKey> transaction;

        public PollyAsyncBackend(TransactionPolicyInfo policy, ITransactionProvider<TKey> transaction)
        {
            this.transaction = transaction;
            this.lockPolicy = Policy
                .HandleResult<bool>(acquired => !acquired)
                .WaitAndRetry(policy.RetryTimeouts);
        }

        private async Task AcquireLock(TKey key) => await lockPolicy.ExecuteAsync(() =>
        {
            return Task.FromResult(transaction.Begin(key));
        });

        private async Task ReleaseLock(TKey key) => await lockPolicy.ExecuteAsync(() =>
        {
            return Task.FromResult(transaction.End(key));
        });

        public Task<Entity<TKey, TValue>> this[TKey key] => Get(key);

        public Task<IEnumerable<TKey>> Keys => throw new NotImplementedException();

        public Task<IEnumerable<Entity<TKey, TValue>>> Entities => throw new NotImplementedException();

        public async Task Delete(TKey key)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Entity<TKey, TValue> entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(TKey key)
        {
            throw new NotImplementedException();
        }

        public Task<Entity<TKey, TValue>> Get(TKey key)
        {
            throw new NotImplementedException();
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

        public Task Save(TKey key, TValue value, bool makeSnapshot = true)
        {
            throw new NotImplementedException();
        }
    }
}