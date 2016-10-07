using System;
using System.Threading.Tasks;
using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Core.Repositories;
using Moq;
using NLog;
using NUnit.Framework;

namespace KFH.ValantInventory.UnitTests.CoreTests
{
    [TestFixture]
    public partial class InventoryRepositoryTests
    {

        [Test]
        public async Task InventoryRepository_AddAsync_AddedReturnsTrue()
        {
            var label = "InventoryRepository_AddAsync_AddedReturnsTrue";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<ILogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup((ir => ir.ReadAsync(item.Label))).Returns(Task.FromResult(default(Common.Models.Inventory)));
            mockDataAccessClient.Setup(ir => ir.CreateAsync(item)).Returns(Task.FromResult(true));

            var result = await repo.AddAsync(item).ConfigureAwait(false);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task InventoryRepository_AddAsync_NotAddedReturnsFalse()
        {
            var label = "InventoryRepository_AddAsync_NotAddedReturnsFalse";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<ILogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup((ir => ir.ReadAsync(item.Label))).Returns(Task.FromResult(item));
            mockDataAccessClient.Setup(ir => ir.CreateAsync(item)).Returns(Task.FromResult(true));

            var result = await repo.AddAsync(item).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task InventoryRepository_AddAsync_NonUtcExpiration_SetToUtc()
        {
            var label = "InventoryRepository_AddAsync_NonUtcExpiration_SetToUtc";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Unspecified),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<ILogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup((ir => ir.ReadAsync(item.Label))).Returns(Task.FromResult(default(Common.Models.Inventory)));
            mockDataAccessClient.Setup(ir => ir.CreateAsync(item)).Returns(Task.FromResult(true));

            var result = await repo.AddAsync(item).ConfigureAwait(false);
            Assert.AreEqual(DateTimeKind.Utc, item.ExpirationDateUtc.Kind);
        }

        [Test]
        public void InventoryRepository_AddAsync_Exception_LoggedRethrown()
        {
            var label = "InventoryRepository_AddAsync_Exception_LoggedRethrown";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var mockLogger = new Mock<ILogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);
            var expectedException = new Exception("Test Exception Thrown");

            var repo = new InventoryRepository(mockFactory, mockLogger.Object);
            mockDataAccessClient.Setup((ir => ir.ReadAsync(item.Label))).Throws(expectedException);

            Assert.Catch<Exception>(() => repo.AddAsync(item).Wait());
            mockLogger.Verify(l => l.Error(expectedException, It.IsAny<string>()));

        }

        private IInventoryDataAccessFactory CreateMockDataAccessFactory(
            Mock<IInventoryDataAccessClient> mockDataAccessClient,
            Mock<IDeletedInventoryQueue> fakeDeletedInventoryQueue,
            Mock<IExpiredInventoryQueue> fakeExpiredInventoryQueue)
        {
            var mockFactory = new Mock<IInventoryDataAccessFactory>();
            mockFactory.Setup(f => f.CreateInventoryDataAcessClient()).Returns(mockDataAccessClient.Object);
            mockFactory.Setup(f => f.CreateDeletedInventoryQueueClient()).Returns(fakeDeletedInventoryQueue.Object);
            mockFactory.Setup(f => f.CreateExpiredInventoryQueueClient()).Returns(fakeExpiredInventoryQueue.Object);

            return mockFactory.Object;
        }
    }
}
