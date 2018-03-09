using System;
using Diffstore;
using Diffstore.DBMS;
using Diffstore.DBMS.Core;
using Diffstore.DBMS.Drivers;

namespace Standalone.Core
{
    public class DynamicDiffstore
    {
        public dynamic Storage { get; }
        public Type KeyType { get; }
        public Type ValueType { get; }
        public SchemaDefinition Schema { get; }

        public DynamicDiffstore(dynamic storage, Type keyType, Type entityType,
            SchemaDefinition schema) =>
            (Storage, KeyType, ValueType, Schema) = 
            (storage, keyType, entityType, schema);
    }

    public static class DynamicDiffstoreBuilder
    {
        public static DynamicDiffstore Create(Options options, SchemaDefinition schema)
        {
            var keyType = TypeResolver.FromName(options.KeyType);
            var valueType = DynamicTypeBuilder.CreateFrom(schema);

            var builderType = typeof(DiffstoreBuilder<,>).MakeGenericType(keyType, valueType);
            dynamic instance = Activator.CreateInstance(builderType);

            switch (options.Store)
            {
                case StorageMethod.InMemory: instance.WithMemoryStorage(); break;
                case StorageMethod.OnDisk: instance.WithDiskStorage("storage"); break;
            }

            instance.WithFileBasedEntities(options.EntityFormat);
            switch (options.Snapshots)
            {
                case SnapshotStorage.LastFirst:
                    instance.WithLastFirstOptimizedSnapshots(); break;
                case SnapshotStorage.SingleFile:
                    instance.WithSingleFileSnapshots(options.SnapshotFormat); break;
            }

            var wrapperType = typeof(EmbeddedDBMS<,>)
                .MakeGenericType(keyType, valueType);
            var transactionProviderType = typeof(ConcurrentTransactionProvider<>)
                .MakeGenericType(keyType);
            var transactionPolicy = TransactionPolicy.FixedRetries(
                options.MaxRetries,
                TimeSpan.FromMilliseconds(options.RetryTimeout)
            );

            var wrapper = Activator.CreateInstance(wrapperType,
                instance.Setup(),
                transactionPolicy,
                Activator.CreateInstance(transactionProviderType));
            return new DynamicDiffstore(wrapper, keyType, valueType, schema);
        }
    }
}