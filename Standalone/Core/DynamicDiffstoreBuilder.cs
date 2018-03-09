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

        public DynamicDiffstore(dynamic storage, Type keyType, Type entityType) =>
            (Storage, KeyType, ValueType) = (storage, keyType, entityType);
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

            instance.WithFileBasedEntities(options.EntityFormat);
            switch(options.Snapshots)
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
            // TODO Make configurable
            var transactionPolicy = TransactionPolicy
                .FixedRetries(5, TimeSpan.FromMilliseconds(1000));
            
            var wrapper = Activator.CreateInstance(wrapperType,
                instance.Setup(), 
                transactionPolicy,
                Activator.CreateInstance(transactionProviderType));
            return new DynamicDiffstore(wrapper, keyType, valueType);
        }
    }
}