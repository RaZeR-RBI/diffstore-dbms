using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Standalone.Core
{
    public static class TypeResolver
    {
        private static Dictionary<string, Type> primitives =
            new Dictionary<string, Type>()
            {
                ["bool"] = typeof(bool),
                ["byte"] = typeof(byte),
                ["sbyte"] = typeof(sbyte),
                ["short"] = typeof(short),
                ["ushort"] = typeof(ushort),
                ["int"] = typeof(int),
                ["uint"] = typeof(uint),
                ["long"] = typeof(long),
                ["ulong"] = typeof(ulong),
                ["float"] = typeof(float),
                ["double"] = typeof(double),
                ["decimal"] = typeof(decimal),
                ["char"] = typeof(char),
                ["string"] = typeof(string)
            };

        private static Regex isList = new Regex(
            "(?<=^List<|IList<)(?<item>.*)(?=>)"
        );

        private static Regex isDictionary = new Regex(
            "(?<=^Dictionary<|IDictionary<)(?<key>.*)(?:,\\s)(?<value>.*)(?=>)"
        );

        public static Type FromName(string name, bool primitivesOnly = false)
        {
            if (primitives.ContainsKey(name))
                return primitives[name];

            if (primitivesOnly)
                throw new ArgumentException("Unsupported type \"{name}\", primitive required");

            if (isList.IsMatch(name))
            {
                var match = isList.Match(name);
                var itemType = FromName(match.Groups["item"].Value);
                return typeof(List<>).MakeGenericType(itemType);
            }

            if (isDictionary.IsMatch(name))
            {
                var match = isDictionary.Match(name);
                var keyType = FromName(match.Groups["key"].Value);
                var valueType = FromName(match.Groups["value"].Value);
                return typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            }

            throw new ArgumentException($"Unsupported type \"{name}\"");
        }
    }
}