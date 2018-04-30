using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Jil;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Standalone.Util
{
    public sealed class JilOutputFormatter : OutputFormatter
    {
        public JilOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;
            var data = Encoding.UTF8.GetBytes(JSON.Serialize(context.Object));
            return response.Body.WriteAsync(data, 0, data.Length);
        }
    }
}