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
        public async Task InventoryRepository_DeleteAsync_DeletedReturnsTrue()
        {
            var label = "InventoryRepository_DeleteAsync_DeletedReturnsTrue";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var mockDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, mockDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(item));
            mockDataAccessClient.Setup(ir => ir.DeleteAsync(label)).Returns(Task.FromResult(true));

            var result = await repo.DeleteAsync(label).ConfigureAwait(false);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task InventoryRepository_DeleteAsync_AddedToDeleteQueue()
        {
            var label = "InventoryRepository_DeleteAsync_AddedToDeleteQueue";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var mockDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, mockDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(item));
            mockDataAccessClient.Setup(ir => ir.DeleteAsync(label)).Returns(Task.FromResult(true));
            mockDeletedInventoryQueue.Setup(q => q.Enqueue(item)).Returns(Task.CompletedTask);

            await repo.DeleteAsync(label).ConfigureAwait(false);
            mockDeletedInventoryQueue.Verify(q => q.Enqueue(item));
        }

        [Test]
        public async Task InventoryRepository_DeleteAsync_NotDeletedReturnsFalse()
        {
            var label = "InventoryRepository_DeleteAsync_NotDeletedReturnsFalse";


            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(default(Common.Models.Inventory)));

            var result = await repo.DeleteAsync(label).ConfigureAwait(false);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task InventoryRepository_DeleteAsync_NotDeletedNotQueued()
        {
            var label = "InventoryRepository_DeleteAsync_NotDeletedReturnsFalse";


            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var mockDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, mockDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Returns(Task.FromResult(default(Common.Models.Inventory)));

            await repo.DeleteAsync(label).ConfigureAwait(false);
            mockDeletedInventoryQueue.Verify(q => q.Enqueue(It.IsAny<Common.Models.Inventory>()), Times.Never);
        }

        [Test]
        public void InventoryRepository_DeleteAsync_Exception_LoggedRethrown()
        {
            var label = "InventoryRepository_DeleteAsync_Exception_LoggedRethrown";

            var mockLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);
            var expectedException = new Exception("Test Exception Thrown");

            var repo = new InventoryRepository(mockFactory, mockLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(label)).Throws(expectedException);

            Assert.Catch<Exception>(() => repo.DeleteAsync(label).Wait());
            mockLogger.Verify(l => l.Error(expectedException, It.IsAny<string>()));

        }


    }
}
