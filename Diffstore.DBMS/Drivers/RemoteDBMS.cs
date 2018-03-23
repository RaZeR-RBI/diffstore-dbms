using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Diffstore.Entities;
using Diffstore.Snapshots;

namespace Diffstore.DBMS.Drivers
{
    public class RemoteDBMS<TKey, TValue> : IDiffstoreDBMS<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        public IEnumerable<TKey> Keys => throw new NotImplementedException();
        private HttpClient client;

        public RemoteDBMS(Uri connectionUri)
        {
            client = new HttpClient {
                BaseAddress = connectionUri
            };
            client.DefaultRequestHeaders.ConnectionClose = false;
            // TODO Add timeouts
        }

        public Task Delete(TKey key)
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

        public Task<IEnumerable<Entity<TKey, TValue>>> GetAll()
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

        public Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshotsBetween(TKey key, long timeStart, long timeEnd)
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

        public void Dispose() => client.Dispose(); 
    }
}