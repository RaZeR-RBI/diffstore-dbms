using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Diffstore.Entities;
using Diffstore.Snapshots;

namespace Diffstore.DBMS.Core
{
    public interface IAsyncBackend<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        IEnumerable<TKey> Keys { get; }
        Task Delete(TKey key);
        Task Delete(Entity<TKey, TValue> entity);
        Task<bool> Exists(TKey key);
        Task<Entity<TKey, TValue>> Get(TKey key);
        Task<IEnumerable<Entity<TKey, TValue>>> GetAll();
        Task<Snapshot<TKey, TValue>> GetFirst(TKey key);
        Task<long> GetFirstTime(TKey key);
        Task<Snapshot<TKey, TValue>> GetLast(TKey key);
        Task<long> GetLastTime(TKey key);
        Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key);
        Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key, int from, int count);
        Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key, long timeStart, long timeEnd);
        Task PutSnapshot(Entity<TKey, TValue> entity, long time);
        Task Save(Entity<TKey, TValue> entity, bool makeSnapshot = true);
        Task Save(TKey key, TValue value, bool makeSnapshot = true);
    }

    public static class AsyncBackend
    {
        public static IAsyncBackend<TKey, TValue> Create<TKey, TValue>(IDiffstore<TKey, TValue> db,
            ITransactionProvider<TKey> provider, TransactionPolicyInfo policy)
            where TKey : IComparable
            where TValue : class, new() =>
                new PollyAsyncBackend<TKey, TValue>(db, policy, provider);
    }
}