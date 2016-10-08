using System;
using System.Threading.Tasks;
using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Core.Repositories;
using Moq;
using NUnit.Framework;

namespace KFH.ValantInventory.UnitTests.CoreTests
{
    [TestFixture]
    public partial class InventoryRepositoryTests
    {

        [Test]
        public async Task InventoryRepository_ExpireAsync_Expired_ReturnsTrue()
        {
            var label = "InventoryRepository_ExpireAsync_Expired_ReturnsTrue";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(1900, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs",
                ExpirationQueued = false
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(item));
            mockDataAccessClient.Setup(ir => ir.UpdateAsync(item)).Returns(Task.FromResult(true));

            var result = await repo.ExpireAsync(label).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Make sure the ExpirationQueued flag was set
            Assert.IsTrue(item.ExpirationQueued);
        }

        [Test]
        public async Task InventoryRepository_ExpireAsync_NotExpired_UpdatesExpirationDate()
        {
            var label = "InventoryRepository_ExpireAsync_NotExpired_UpdatesExpirationDate";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3000, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs",
                ExpirationQueued = false
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(item));
            mockDataAccessClient.Setup(ir => ir.UpdateAsync(item)).Returns(Task.FromResult(true));

            var result = await repo.ExpireAsync(label).ConfigureAwait(false);
            Assert.IsTrue(result);

            // Make sure expiration date was set in the past
            Assert.Less(item.ExpirationDateUtc, DateTime.UtcNow);
        }

        [Test]
        public async Task InventoryRepository_ExpireAsync_Expired_AddedToExpireQueue()
        {
            var label = "InventoryRepository_ExpireAsync_Expired_AddedToExpireQueue";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var mockExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                mockExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(item));
            mockDataAccessClient.Setup(ir => ir.UpdateAsync(item)).Returns(Task.FromResult(true));
            mockExpiredInventoryQueue.Setup(q => q.Enqueue(item)).Returns(Task.CompletedTask);

            await repo.ExpireAsync(label).ConfigureAwait(false);
            mockExpiredInventoryQueue.Verify(q => q.Enqueue(item));
        }

        [Test]
        public async Task InventoryRepository_ExpireAsync_AlreadyQueued_ReturnsTrue()
        {
            var label = "InventoryRepository_ExpireAsync_AlreadyQueued_ReturnsTrue";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3000, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs",
                ExpirationQueued = true
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(item));

            var result = await repo.ExpireAsync(label).ConfigureAwait(false);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task InventoryRepository_ExpireAsync_AlreadyQueued_NotReQueued()
        {
            var label = "InventoryRepository_ExpireAsync_AlreadyQueued_NotReQueued";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3000, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs",
                ExpirationQueued = true
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var mockExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                mockExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(item));

            await repo.ExpireAsync(label).ConfigureAwait(false);
            mockExpiredInventoryQueue.Verify(q => q.Enqueue(It.IsAny<Common.Models.Inventory>()), Times.Never);
        }

        [Test]
        public async Task InventoryRepository_DeleteAsync_NotFoundReturnsFalse()
        {
            var label = "InventoryRepository_DeleteAsync_NotFoundReturnsFalse";


            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(default(Common.Models.Inventory)));

            var result = await repo.ExpireAsync(label).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [Test]
        public void InventoryRepository_ExpireAsync_Exception_LoggedRethrown()
        {
            var label = "InventoryRepository_ExpireAsync_Exception_LoggedRethrown";

            var mockLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);
            var expectedException = new Exception("Test Exception Thrown");

            var repo = new InventoryRepository(mockFactory, mockLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Throws(expectedException);

            Assert.Catch<Exception>(() => repo.ExpireAsync(label).Wait());
            mockLogger.Verify(l => l.Error(expectedException, It.IsAny<string>()));

        }


    }
}
