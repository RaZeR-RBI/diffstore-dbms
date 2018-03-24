using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diffstore;
using Diffstore.DBMS;
using Diffstore.DBMS.Core;
using Diffstore.DBMS.Core.Exceptions;
using Diffstore.Entities;
using Moq;
using Xunit;

namespace Tests.Diffstore.DBMS.Drivers
{
    public class EmbeddedDBMSTest
    {
        private IDiffstore<int, SampleData> db = new DiffstoreBuilder<int, SampleData>()
            .WithMemoryStorage()
            .WithFileBasedEntities(FileFormat.Binary)
            .WithSingleFileSnapshots(FileFormat.Binary)
            .Setup();

        private const int EntityStartId = 1;
        private const int EntityEndId = 10;

        private TransactionPolicyInfo policy =
            TransactionPolicy.SingleRetry(TimeSpan.FromMilliseconds(100));



        [Fact]
        public async Task ShouldReturnWhatHasBeenSavedAsync()
        {
            var transactionProvider = TransactionProvider.OfType<int>();
            var backend = DiffstoreDBMS.Embedded(db, transactionProvider, policy);
            var data = CreateSampleData();
            var keys = data.Select(e => e.Key);

            Assert.Empty(await backend.Keys());
            await SaveDataAsync(data, backend);

            Assert.Equal(keys, await backend.Keys());
            var savedData = await backend.GetAll();
            Assert.Equal(data, savedData);
        }

        [Fact]
        public async Task ShouldAllowMultipleReadsAndThrowOnWriteAttemptAsync()
        {
            var transactionProvider = SimulatedReadLock<int>();
            var backend = DiffstoreDBMS.Embedded(db, transactionProvider, policy);
            var data = CreateSampleData();
            var firstKey = data.First().Key;
            var lastKey = data.Last().Key;
            var readLockedKeys = new[] { firstKey, lastKey };
            await SaveDataAsync(data, backend);

            var firstEntity = await backend.Get(firstKey);
            var firstEntityToo = await backend.Get(firstKey);
            var lastEntity = await backend.Get(lastKey);

            Assert.Equal(readLockedKeys, transactionProvider.InRead);
            Assert.Empty(transactionProvider.InWrite);
            Assert.Equal(firstEntity, firstEntityToo);            
            await Assert.ThrowsAsync<ResourceIsBusyException>(() => backend.Save(firstEntity));
        }

        [Fact]
        public async Task ShouldAllowSingleWriteAndThrowOnAnyAccessAttemptAsync()
        {
            var transactionProvider = SimulatedWriteLock<int>();
            var backend = DiffstoreDBMS.Embedded(db, transactionProvider, policy);
            var data = CreateSampleData();
            await SaveDataAsync(data, backend);

            await Assert.ThrowsAsync<ResourceIsBusyException>(
                () => backend.Get(data.First().Key)
            );
            await Assert.ThrowsAsync<ResourceIsBusyException>(
                () => backend.Save(data.First())
            );
        }


        private ITransactionProvider<T> SimulatedReadLock<T>()
            where T : IComparable
        {
            var mock = new Mock<ConcurrentTransactionProvider<T>>() { CallBase = true }
                .As<ITransactionProvider<T>>();
            mock.Setup(m => m.EndRead(It.IsAny<T>())).Returns(true);
            return mock.Object;
        }

        private ITransactionProvider<T> SimulatedWriteLock<T>()
            where T : IComparable
        {
            var mock = new Mock<ConcurrentTransactionProvider<T>>() { CallBase = true }
                .As<ITransactionProvider<T>>();
            mock.Setup(m => m.EndWrite(It.IsAny<T>())).Returns(true);
            return mock.Object;
        }

        private List<Entity<int, SampleData>> CreateSampleData() =>
            Enumerable.Range(EntityStartId, EntityEndId)
                .Select(i => Entity.Create(i, new SampleData($"{i}")))
                .ToList();

        private async Task SaveDataAsync(IEnumerable<Entity<int, SampleData>> data,
            IDiffstoreDBMS<int, SampleData> backend) =>
            await Task.WhenAll(data.Select(e => backend.Save(e)).ToArray());
    }
}
