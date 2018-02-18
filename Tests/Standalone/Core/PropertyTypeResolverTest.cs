using System;
using System.Collections.Generic;
using System.Linq;
using Standalone.Core;
using Xunit;

namespace Tests.Standalone.Core
{
    public class PropertyTypeResolverTest
    {
        [Fact]
        public void ShouldResolvePrimitives()
        {
            var expected = new Dictionary<string, Type>()
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
                ["char"] = typeof(char),
                ["string"] = typeof(string)
            };

            var actual = expected.Keys
                .ToDictionary(k => k, PropertyTypeResolver.FromName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldResolveCollections()
        {
            var expected = new Dictionary<string, Type>()
            {
                ["List<int>"] = typeof(List<int>),
                ["Dictionary<string, bool>"] = typeof(Dictionary<string, bool>),
            };

            var actual = expected.Keys
                .ToDictionary(k => k, PropertyTypeResolver.FromName);

            Assert.Equal(expected, actual);
        }
    }
}