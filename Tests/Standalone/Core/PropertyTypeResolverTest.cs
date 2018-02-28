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
                ["float"] = typeof(float),
                ["double"] = typeof(double),
                ["decimal"] = typeof(decimal),
                ["char"] = typeof(char),
                ["string"] = typeof(string)
            };

            var actual = expected.Keys
                .ToDictionary(k => k, k => TypeResolver.FromName(k));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldResolveCollections()
        {
            // Note: we're caring about the instantiable type here,
            // that's why IList becomes List and IDictionary becomes Dictionary
            var expected = new Dictionary<string, Type>()
            {
                ["IList<int>"] = typeof(List<int>),
                ["List<int>"] = typeof(List<int>),
                ["IDictionary<string, bool>"] = typeof(Dictionary<string, bool>),
                ["Dictionary<string, bool>"] = typeof(Dictionary<string, bool>),
            };

            var actual = expected.Keys
                .ToDictionary(k => k, k => TypeResolver.FromName(k));

            Assert.Equal(expected, actual);
        }
    }
}