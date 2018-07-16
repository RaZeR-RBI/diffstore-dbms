using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Jil;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace Standalone.Util
{
    public class JilInputFormatter : InputFormatter
    {
        public JilInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        }

#pragma warning disable CS0168
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var type = context.ModelType;

            var data = await new StreamReader(request.Body).ReadToEndAsync();
            if (type == typeof(string)) return InputFormatterResult.Success(data);

            try
            {
                return InputFormatterResult.Success(JSON.Deserialize(data, type));
            } catch (Exception ex) {
                return InputFormatterResult.Failure();
            }
        }
#pragma warning restore CS0168
    }
}