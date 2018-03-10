using Nancy;
using Nancy.Bootstrapper;
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

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => {
                ctx.Response.Headers["Content-Type"] = "application/json";
            });
        }
    }
}