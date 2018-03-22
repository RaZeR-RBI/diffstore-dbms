using Nancy;
using Nancy.ErrorHandling;
using static Standalone.Nancy.Util;

namespace Standalone.Nancy
{
    public class NotFoundStatusCodeHandler : IStatusCodeHandler
    {
        private DefaultStatusCodeHandler defaultHandler;

        public NotFoundStatusCodeHandler(DefaultStatusCodeHandler defaultHandler) =>
            this.defaultHandler = defaultHandler;

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            if (ResponseMayHaveBody(context.Request.Method))
                defaultHandler.Handle(statusCode, context);
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) =>
            statusCode == HttpStatusCode.NotFound;
    }
}