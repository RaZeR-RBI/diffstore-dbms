using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jil;
using Nancy;
using Nancy.Responses.Negotiation;

namespace Standalone.Nancy
{
    public class JilSerializer : ISerializer
    {
        public static Jil.Options Options = new Jil.Options(
                includeInherited: true,
                serializationNameFormat: SerializationNameFormat.CamelCase
            );

        private static IReadOnlyCollection<string> _extensions =
            new List<string> { "json" };
        public IEnumerable<string> Extensions => _extensions;

        public bool CanSerialize(MediaRange mediaRange) =>
            mediaRange.Matches("application/json");

        public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
        {
            var data = Encoding.UTF8.GetBytes(JSON.Serialize(model, Options));
            outputStream.Write(data, 0, data.Length);
        }
    }
}