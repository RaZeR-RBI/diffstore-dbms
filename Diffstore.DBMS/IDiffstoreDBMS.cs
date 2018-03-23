using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Diffstore.DBMS.Core;
using Diffstore.DBMS.Drivers;
using Diffstore.Entities;
using Diffstore.Snapshots;

namespace Diffstore.DBMS
{
    public interface IDiffstoreDBMS<TKey, TValue> : IDisposable
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
        Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshotsBetween(TKey key, long timeStart, long timeEnd);
        Task PutSnapshot(Entity<TKey, TValue> entity, long time);
        Task Save(Entity<TKey, TValue> entity, bool makeSnapshot = true);
        Task Save(TKey key, TValue value, bool makeSnapshot = true);
    }

    public static class DiffstoreDBMS
    {
        public static IDiffstoreDBMS<TKey, TValue> Embedded<TKey, TValue>(
            IDiffstore<TKey, TValue> db,
            ITransactionProvider<TKey> provider, TransactionPolicyInfo policy)
            where TKey : IComparable
            where TValue : class, new() =>
                new EmbeddedDBMS<TKey, TValue>(db, policy, provider);

        public static IDiffstoreDBMS<TKey, TValue> Remote<TKey, TValue>(
            Uri connectionUri)
        where TKey : IComparable
        where TValue : class, new() =>
           new RemoteDBMS<TKey, TValue>(connectionUri);
    }
}