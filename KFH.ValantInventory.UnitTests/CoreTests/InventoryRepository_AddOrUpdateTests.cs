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
        public async Task InventoryRepository_AddOrUpdateAsync_Updates_ReturnsFalse()
        {
            var label = "InventoryRepository_AddOrUpdateAsync_Updates_ReturnsFalse";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup((ir => ir.ReadAsync(item.Label))).Returns(Task.FromResult(item));
            mockDataAccessClient.Setup(ir => ir.UpdateAsync(item)).Returns(Task.FromResult(true));

            var result = await repo.AddOrUpdateAsync(item).ConfigureAwait(false);
            Assert.IsFalse(result);

            // Make sure update is called
            mockDataAccessClient.Verify(ir => ir.UpdateAsync(item),Times.Once);
            
        }

        [Test]
        public async Task InventoryRepository_AddOrUpdateAsync_Adds_ReturnsTrue()
        {
            var label = "InventoryRepository_AddOrUpdateAsync_Adds_ReturnsTrue";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup(ir => ir.ReadAsync(item.Label)).Returns(Task.FromResult(default(Common.Models.Inventory)));
            mockDataAccessClient.Setup(ir => ir.CreateAsync(item)).Returns(Task.FromResult(true));

            var result = await repo.AddOrUpdateAsync(item).ConfigureAwait(false);
            Assert.True(result);
        }

        [Test]
        public async Task InventoryRepository_AddOrUpdateAsync_NonUtcExpiration_SetToUtc()
        {
            var label = "InventoryRepository_AddOrUpdateAsync_NonUtcExpiration_SetToUtc";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Unspecified),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);

            var repo = new InventoryRepository(mockFactory, stubLogger.Object);
            mockDataAccessClient.Setup((ir => ir.ReadAsync(item.Label))).Returns(Task.FromResult(item));
            mockDataAccessClient.Setup(ir => ir.UpdateAsync(item)).Returns(Task.FromResult(true));

            await repo.AddOrUpdateAsync(item).ConfigureAwait(false);
            Assert.AreEqual(DateTimeKind.Utc, item.ExpirationDateUtc.Kind);
        }

        [Test]
        public void InventoryRepository_AddOrUpdateAsync_Exception_LoggedRethrown()
        {
            var label = "InventoryRepository_AddOrUpdateAsync_Exception_LoggedRethrown";
            var item = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var mockLogger = new Mock<IInventoryLogger>();
            var mockDataAccessClient = new Mock<IInventoryDataAccessClient>();
            var stubDeletedInventoryQueue = new Mock<IDeletedInventoryQueue>();
            var stubExpiredInventoryQueue = new Mock<IExpiredInventoryQueue>();
            var mockFactory = CreateMockDataAccessFactory(mockDataAccessClient, stubDeletedInventoryQueue,
                stubExpiredInventoryQueue);
            var expectedException = new Exception("Test Exception Thrown");

            var repo = new InventoryRepository(mockFactory, mockLogger.Object);
            mockDataAccessClient.Setup((ir => ir.ReadAsync(item.Label))).Throws(expectedException);

            Assert.Catch<Exception>(() => repo.AddOrUpdateAsync(item).Wait());
            mockLogger.Verify(l => l.Error(expectedException, It.IsAny<string>()));

        }

    }
}
