using Jil;
using Nancy;
using Standalone.Core;

namespace Standalone.Nancy
{
    public class MainModule : NancyModule
    {
        private DynamicDiffstore _storage;
        public MainModule(DynamicDiffstore storage)
        {
            _storage = storage;
            Get("/", d => JSON.Serialize(_storage.Schema));

            // TODO Routes for every action
        }
    }
}