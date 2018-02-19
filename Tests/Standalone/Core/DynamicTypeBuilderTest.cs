using System;
using System.Linq;
using Diffstore.Snapshots;
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

        [Fact]
        public void ShouldAddAttributesIfSpecified()
        {
            var schema = new SchemaDefinition()
                .WithField("SavedField", "string")
                .WithField("OnlyActualField", "string", ignoreChanges: true)
                .WithField("IgnoredField", "string", doNotSave: true);
            
            var type = DynamicTypeBuilder.CreateFrom(schema);
            var properties = type.GetProperties();

            Assert.Equal(3, properties.Length);
            Assert.Contains(properties, p =>
                p.Name == "SavedField" && 
                !p.GetCustomAttributes(false).Any());
            Assert.Contains(properties, p =>
                p.Name == "OnlyActualField" &&
                p.GetCustomAttributes(false).Length == 1 &&
                Attribute.IsDefined(p, typeof(IgnoreChangesAttribute)));
            Assert.Contains(properties, p =>
                p.Name == "IgnoredField" &&
                p.GetCustomAttributes(false).Length == 1 &&
                Attribute.IsDefined(p, typeof(IgnoreChangesAttribute)));
        }
    }
}