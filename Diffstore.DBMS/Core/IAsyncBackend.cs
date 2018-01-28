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
        Task<IEnumerable<TKey>> Keys { get; }
        Task<IEnumerable<Entity<TKey, TValue>>> Entities { get; }
        Task Delete(TKey key);
        Task Delete(Entity<TKey, TValue> entity);
        Task<bool> Exists(TKey key);
        Task<Entity<TKey, TValue>> Get(TKey key);
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
}