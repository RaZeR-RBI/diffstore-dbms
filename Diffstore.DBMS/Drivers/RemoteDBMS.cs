using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Diffstore.DBMS.Core;
using Diffstore.Entities;
using Diffstore.Snapshots;
using Jil;

namespace Diffstore.DBMS.Drivers
{
    public class RemoteDBMS<TKey, TValue> : IDiffstoreDBMS<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        private static Jil.Options options = new Jil.Options(
            includeInherited: true,
            serializationNameFormat: SerializationNameFormat.CamelCase
        );

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

        public async Task<Entity<TKey, TValue>> Get(TKey key)
        {
            var response = await client.GetAsync($"entities/{key}");
            var entity = await ParseResponse<EntityExt<TKey, TValue>>(response);
            return entity.Create();
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

        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key)
        {
            var response = await client.GetAsync($"snapshots/{key}");
            var result = await ParseResponse<IList<SnapshotExt<TKey, TValue>>>(response);
            return result.Select(s => s.Create());
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
            CheckResponse(await client.PostAsync(
                "entities",
                ToJson(new SaveRequest(entity, makeSnapshot))
            ));

        public async Task Save(TKey key, TValue value, bool makeSnapshot = true) =>
            await Save(Entity.Create(key, value), makeSnapshot);




        public void Dispose() => client.Dispose();

        private HttpContent ToJson<T>(T value) => new StringContent(
            JSON.Serialize(value, options),
            Encoding.UTF8,
            "application/json"
            );

        private async Task<T> FromJson<T>(HttpContent content) =>
            JSON.Deserialize<T>(await content.ReadAsStringAsync(), options);

        private void CheckResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;
            else throw ResponseException(response);
        }

        private async Task<T> ParseResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return await FromJson<T>(response.Content);
            else throw ResponseException(response);
        }

        private Exception ResponseException(HttpResponseMessage response)
        {
            // TODO Handle specific error messages
            var message = response.Content.ReadAsStringAsync().Result;
            return new Exception($"{response.StatusCode}: {message}");
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