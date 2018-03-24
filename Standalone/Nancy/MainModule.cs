using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Jil;
using Nancy;
using Nancy.Extensions;
using Standalone.Core;
using Standalone.Model;

namespace Standalone.Nancy
{
    public class MainModule : NancyModule
    {
        private DynamicDiffstore _backend;
        public MainModule(DynamicDiffstore backend)
        {
            _backend = backend;
            var db = backend.Storage;
            Get("/", d => _backend.Schema);
            Head("/entities/{id}", async (p) => {
                await db.Exists(p.id);
                return null;
            });
            Get("/entities/{id}", async (p) => await db.Get(p.id));
            Get("/keys", async (_) => await db.Keys());
            Get("/entities", async (_) => await db.GetAll());
            Post("/entities", async (_) =>
            {
                var request = GetRequestBody(SaveRequest.For(backend));
                await db.Save(request.Key, request.Value, request.MakeSnapshot);
                return null;
            });
            Delete("/entities/{id}", async (p) => {
                await db.Delete(p.id);
                return null;
            });

            Get("/snapshots/{id}", async (p) =>
            {
                var query = this.Request.Query;
                if (query.from.HasValue && query.count.HasValue)
                    return await db.GetSnapshots(p.id, query.from, query.count);
                if (query.timeStart.HasValue && query.timeEnd.HasValue)
                    return await db.GetSnapshotsBetween(p.id, query.timeStart, query.timeEnd);
                return await db.GetSnapshots(p.id);
            });
            Get("/snapshots/{id}/firstTime", async (p) => await db.GetFirstTime(p.id));
            Get("/snapshots/{id}/first", async (p) => await db.GetFirst(p.id));
            Get("/snapshots/{id}/lastTime", async (p) => await db.GetLastTime(p.id));
            Get("/snapshots/{id}/last", async (p) => await db.GetLast(p.id));
            Put("/snapshots", async (_) =>
            {
                var request = GetRequestBody(PutSnapshotRequest.For(backend));
                await db.PutSnapshot(request.State.AsEntity(), request.Time);
                return null;
            });
        }

        private dynamic GetRequestBody(Type type) =>
            JSON.Deserialize(AsString(this.Request.Body), type, JilSerializer.Options);

        // TODO Move to extensions
        private static string AsString(Stream stream, Encoding encoding = null)
        {
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8, true, 4096, true))
            {
                if (stream.CanSeek)
                {
                    var initialPosition = stream.Position;
                    stream.Position = 0;
                    var content = reader.ReadToEnd();
                    stream.Position = initialPosition;
                    return content;
                }
                return string.Empty;
            }
        }
    }
}