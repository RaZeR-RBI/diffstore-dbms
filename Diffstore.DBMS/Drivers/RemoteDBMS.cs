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

        public async Task Delete(TKey key) =>
            CheckResponse(await client.DeleteAsync($"entities/{key}"));

        public async Task Delete(Entity<TKey, TValue> entity) =>
            await Delete(entity.Key);

        public async Task<bool> Exists(TKey key)
        {
            var message = new HttpRequestMessage(HttpMethod.Head, $"entities/{key}");
            var response = await client.SendAsync(message);
            if (response.IsSuccessStatusCode) return true;
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            throw ResponseException(response);
        }

        public async Task<Entity<TKey, TValue>> Get(TKey key)
        {
            var response = await client.GetAsync($"entities/{key}");
            var entity = await ParseResponse<EntityExt<TKey, TValue>>(response);
            return entity.Create();
        }

        public async Task<IEnumerable<Entity<TKey, TValue>>> GetAll()
        {
            var response = await client.GetAsync("entities");
            var entities = await ParseResponse<IList<EntityExt<TKey, TValue>>>(response);
            return entities.Select(e => e.Create());
        }

        public async Task<Snapshot<TKey, TValue>> GetFirst(TKey key)
        {
            var response = await client.GetAsync($"snapshots/{key}/first");
            var snapshot = await ParseResponse<SnapshotExt<TKey, TValue>>(response);
            return snapshot.Create();
        }

        public async Task<long> GetFirstTime(TKey key) =>
            await ParseResponse<long>(await client.GetAsync($"snapshots/{key}/firstTime"));

        public async Task<Snapshot<TKey, TValue>> GetLast(TKey key)
        {
            var response = await client.GetAsync($"snapshots/{key}/last");
            var snapshot = await ParseResponse<SnapshotExt<TKey, TValue>>(response);
            return snapshot.Create();
        }

        public async Task<long> GetLastTime(TKey key) =>
            await ParseResponse<long>(await client.GetAsync($"snapshots/{key}/lastTime"));

        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key)
        {
            var response = await client.GetAsync($"snapshots/{key}");
            var result = await ParseResponse<IList<SnapshotExt<TKey, TValue>>>(response);
            return result.Select(s => s.Create());
        }

        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(TKey key, int from, int count)
        {
            var response = await client.GetAsync(
                $"snapshots/{key}?from={from}&count={count}"
            );
            var result = await ParseResponse<IList<SnapshotExt<TKey, TValue>>>(response);
            return result.Select(s => s.Create());
        }

        public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshotsBetween(TKey key, long timeStart, long timeEnd)
        {
            var response = await client.GetAsync(
                $"snapshots/{key}?timeStart={timeStart}&timeEnd={timeEnd}"
            );
            var result = await ParseResponse<IList<SnapshotExt<TKey, TValue>>>(response);
            return result.Select(s => s.Create());
        }

        public async Task PutSnapshot(Entity<TKey, TValue> entity, long time) =>
            CheckResponse(await client.PutAsync(
                "snapshots",
                ToJson(Snapshot.Create(time, entity))
            ));

        public async Task Save(Entity<TKey, TValue> entity, bool makeSnapshot = true) =>
            CheckResponse(await client.PostAsync(
                "entities",
                ToJson(new SaveRequest(entity, makeSnapshot))
            ));

        public async Task Save(TKey key, TValue value, bool makeSnapshot = true) =>
            await Save(Entity.Create(key, value), makeSnapshot);

        public async Task<IEnumerable<TKey>> Keys()
        {
            var response = await client.GetAsync("keys");
            var keys = await ParseResponse<IList<TKey>>(response);
            return keys;
        }



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