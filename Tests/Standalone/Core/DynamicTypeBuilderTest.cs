using System.Linq;
using Standalone.Core;
using Xunit;

namespace Tests.Standalone.Core
{
    public class DynamicTypeBuilderTest
    {
        [Fact]
        public void ShouldCreatePropertiesAndDefaultConstructor()
        {
            var schema = new SchemaDefinition()
                .WithField("StringField", "string")
                .WithField("IntField", "int");

            var type = DynamicTypeBuilder.CreateFrom(schema);
            var properties = type.GetProperties();

            Assert.True(type.GetConstructors()
                .Any(c => !c.GetParameters().Any()));
            Assert.Equal(2, properties.Length);
            Assert.Contains(properties, p =>
                p.Name == "StringField" &&
                p.PropertyType == typeof(string));
            Assert.Contains(properties, p =>
                p.Name == "IntField" &&
                p.PropertyType == typeof(int));
        }
    }
}