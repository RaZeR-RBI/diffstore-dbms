using System;
using System.Linq;
using Diffstore.Entities;
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

            Assert.Contains(type.GetConstructors(), c => !c.GetParameters().Any());
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
                .WithField("IgnoredField", "string", doNotPersist: true);

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
                Attribute.IsDefined(p, typeof(DoNotPersistAttribute)));
        }

        [Fact]
        public void ShouldOverrideEqualsAndHashCode()
        {
            var schema = new SchemaDefinition()
                .WithField("StringField", "string")
                .WithField("IntField", "int");

            var type = DynamicTypeBuilder.CreateFrom(schema);
            var stringField = type.GetProperty("StringField");
            var intField = type.GetProperty("IntField");

            var objectOne = Activator.CreateInstance(type);
            var objectOneToo = Activator.CreateInstance(type);
            var objectTwo = Activator.CreateInstance(type);
            stringField.SetValue(objectOne, "One");
            stringField.SetValue(objectOneToo, "One");
            intField.SetValue(objectOne, 1);
            intField.SetValue(objectOneToo, 1);
            intField.SetValue(objectTwo, 2000);

            Assert.Equal(objectOne, objectOneToo);
            Assert.Equal(objectOne.GetHashCode(), objectOneToo.GetHashCode());
            Assert.NotEqual(objectOne, objectTwo);
            Assert.NotEqual(objectOne.GetHashCode(), objectTwo.GetHashCode());
        }
    }
}