using System;
using System.Threading.Tasks;
using Diffstore.DBMS.Core.Exceptions;
using Jil;
using Microsoft.AspNetCore.Mvc;
using Standalone.Core;
using Standalone.Model;
using Standalone.Util;

namespace Standalone.Controllers
{
    [Route("/")]
    public class MainController
    {
        private DynamicDiffstore _backend;
        private dynamic db;
        private Type saveRequest;

        public MainController(DynamicDiffstore backend) =>
            (_backend, db, saveRequest) = 
            (backend, backend.Storage, SaveRequest.For(backend));

        [HttpGet]
        public SchemaDefinition GetSchema() => _backend.Schema;

        [HttpHead("entities/{id}")]
        public async Task<IActionResult> Exists(string id) {
            var exists = await db.Exists(ParseId(id));
            if (exists) {
                return new OkResult();
            } else return new NotFoundResult();
        }

        [HttpGet("entities/{id}")]
        public async Task<dynamic> GetEntity(string id) => await db.Get(ParseId(id));

        [HttpGet("keys")]
        public async Task<dynamic> GetKeys() => await db.Keys();

        [HttpGet("entities")]
        public async Task<dynamic> GetEntities() => await db.GetAll();

        [HttpPost("entities")]
        public async Task SaveEntity([FromBody] string data)
        {
            var request = GetRequestBody(saveRequest, data);
            await db.Save(request.Key, request.Value, request.MakeSnapshot);
        }

        [HttpDelete("entities/{id}")]
        public async Task DeleteEntity(string id) => await db.Delete(ParseId(id));

        [HttpGet("snapshots/{id}")]
        public async Task<dynamic> GetSnapshots(string id, [FromQuery] GetSnapshotRequest request)
        {
            var _id = ParseId(id);
            if (request.IsPage()) {
                return await Handle(() => 
                    db.GetSnapshots(_id, request.From.Value, request.Count.Value));
            } else if (request.IsTimeframe()) {
                return await Handle(() => 
                    db.GetSnapshotsBetween(_id, request.TimeStart.Value, request.TimeEnd.Value));
            }
            return await Handle(() => (dynamic)db.GetSnapshots(_id));
        }

        [HttpGet("snapshots/{id}/firstTime")]
        public async Task<long> GetFirstTime(string id) =>
            await db.GetFirstTime(ParseId(id));

        [HttpGet("snapshots/{id}/lastTime")]
        public async Task<long> GetLastTime(string id) =>
            await db.GetLastTime(ParseId(id));

        [HttpGet("snapshots/{id}/first")]
        public async Task<dynamic> GetFirst(string id) =>
            await db.GetFirst(ParseId(id));

        [HttpGet("snapshots/{id}/last")]
        public async Task<dynamic> GetLast(string id) =>
            await db.GetLast(ParseId(id));

        [HttpPut("snapshots")]
        public async Task PutSnapshot([FromBody] string data)
        {
            var request = GetRequestBody(PutSnapshotRequest.For(_backend), data);
            await db.PutSnapshot(request.State.AsEntity(), request.Time);
        }

        private async Task<IActionResult> Handle(Func<dynamic> response) {
            try
            {
                return new OkObjectResult(await response());
            } catch (Exception ex) {
                if (ex is EntityNotFoundException || ex is SnapshotNotFoundException) {
                    return new NotFoundResult();
                }
                throw;
            }
        }

        private dynamic GetRequestBody(Type type, string body) =>
            JSON.Deserialize(body, type);

        // TODO: Find a better way (ulong is not supported)
        private dynamic ParseId(string id) {
            if (long.TryParse(id, out long number)) {
                return number;
            }
            return id;
        }
    }
}