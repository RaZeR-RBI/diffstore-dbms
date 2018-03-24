using System;
using System.Linq;
using System.Threading.Tasks;
using Diffstore;
using Diffstore.Entities;
using Standalone;
using Standalone.Core;
using Xunit;

namespace Tests.Standalone.Core
{
    public class DynamicDiffstoreBuilderTest
    {
        [Fact]
        public async Task ShouldCreateValidInstanceAsync()
        {
            var options = new Options()
            {
                KeyType = "long",
                Store = StorageMethod.InMemory,
                EntityFormat = FileFormat.JSON,
                SnapshotFormat = FileFormat.JSON,
                Snapshots = SnapshotStorage.SingleFile,
            };
            var schema = new SchemaDefinition()
                .WithField("StringField", "string")
                .WithField("IntField", "int");

            var instance = DynamicDiffstoreBuilder.Create(options, schema);
            var db = instance.Storage;

            Assert.Empty(await db.Keys());
            var sampleEntity = CreateEntity(instance.KeyType, instance.ValueType,
                1L, "test", 1337);
            await db.Save(sampleEntity, true);
            Assert.Single(await db.Keys());
        }

        private static dynamic CreateEntity(Type keyType, Type valueType, object key,
            string stringFieldValue, int intFieldValue)
        {
            var stringProp = valueType.GetProperty("StringField");
            var intProp = valueType.GetProperty("IntField");
            var instance = Activator.CreateInstance(valueType);
            stringProp.SetValue(instance, stringFieldValue);
            intProp.SetValue(instance, intFieldValue);
            return Activator.CreateInstance(
                typeof(Entity<,>).MakeGenericType(keyType, valueType),
                key,
                instance
            );
        }
    }
}