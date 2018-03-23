using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Diffstore.Entities;
using Diffstore.Snapshots;
using Jil;

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
            client = new HttpClient
            {
                BaseAddress = connectionUri
            };
            client.DefaultRequestHeaders.ConnectionClose = false;
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
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

        public async Task Save(Entity<TKey, TValue> entity, bool makeSnapshot = true) =>
            await client.PostAsync("entities",
                ToJson(new SaveRequest(entity, makeSnapshot)))
                .ContinueWith(Handle);

        public Task Save(TKey key, TValue value, bool makeSnapshot = true) =>
            Save(Entity.Create(key, value), makeSnapshot);

        public void Dispose() => client.Dispose();

        private HttpContent ToJson<T>(T value) => new StringContent(JSON.Serialize(value));

        private void Handle(Task<HttpResponseMessage> task)
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                var response = task.Result;
                if (response.IsSuccessStatusCode) return;

                // TODO Handle specific errors
                throw new Exception(response.Content.ToString());
            }
            else
            {
                if (task.Exception.InnerExceptions.Count == 1)
                    throw task.Exception.InnerException;
                else
                    throw task.Exception;
            }
        }

        public class SaveRequest
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public bool MakeSnapshot { get; set; }

            public SaveRequest() { }

            public SaveRequest(Entity<TKey, TValue> entity, bool makeSnapshot) =>
                (Key, Value, MakeSnapshot) = (entity.Key, entity.Value, makeSnapshot);
        }
    }
}