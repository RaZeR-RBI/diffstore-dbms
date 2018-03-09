using Nancy;
using Nancy.TinyIoc;
using Standalone;
using Standalone.Core;

namespace Standalone.Nancy
{
    public class MainBootstrapper : DefaultNancyBootstrapper
    {
        private Options _options;
        private SchemaDefinition _schema;

        public MainBootstrapper(Options options, SchemaDefinition schema) =>
            (_schema, _options) = (schema, options);

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register(
                typeof(DynamicDiffstore), 
                DynamicDiffstoreBuilder.Create(_options, _schema)
            );
        }
    }
}