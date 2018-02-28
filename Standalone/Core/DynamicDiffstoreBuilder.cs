using System;
using Diffstore;

namespace Standalone.Core
{
    public class DynamicDiffstore
    {
        public dynamic Storage { get; }
        public Type KeyType { get; }
        public Type EntityType { get; }

        public DynamicDiffstore(dynamic storage, Type keyType, Type entityType) =>
            (Storage, KeyType, EntityType) = (storage, keyType, entityType);
    }

    public static class DynamicDiffstoreBuilder
    {
        public static DynamicDiffstore Create(Options options, SchemaDefinition schema)
        {
            var keyType = TypeResolver.FromName(options.KeyType);
            var valueType = DynamicTypeBuilder.CreateFrom(schema);

            var builderType = typeof(DiffstoreBuilder<,>).MakeGenericType(keyType, valueType);
            dynamic instance = Activator.CreateInstance(builderType);
            
            switch(options.Store)
            {
                case StorageMethod.InMemory: instance.WithMemoryStorage(); break;
                case StorageMethod.OnDisk: instance.WithDiskStorage("storage"); break;
            }

            instance.WithFileBasedEntities(options.EntityFormat, null, null);

            // TODO
            throw new NotImplementedException();
            // return new DynamicDiffstore(instance.Setup(), keyType, valueType);
        }
    }
}