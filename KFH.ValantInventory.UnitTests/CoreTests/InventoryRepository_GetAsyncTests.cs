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
        public async Task InventoryRepository_GetAsync_Found_ReturnsItem()
        {
            var label = "InventoryRepository_GetAsync_Found_ReturnsItem";
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

            var result = await repo.GetAsync(item.Label).ConfigureAwait(false);
            Assert.AreSame(item, result);
            
        }

        [Test]
        public async Task InventoryRepository_GetAsync_NotFound_ReturnsNull()
        {
            var label = "InventoryRepository_GetAsync_NotFound_ReturnsNull";
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
            mockDataAccessClient.Setup((ir => ir.ReadAsync(item.Label))).Returns(Task.FromResult(default(Common.Models.Inventory)));

            var result = await repo.GetAsync(item.Label).ConfigureAwait(false);
            Assert.IsNull(result);
        }

        [Test]
        public void InventoryRepository_GetAsync_Exception_LoggedRethrown()
        {
            var label = "InventoryRepository_GetAsync_Exception_LoggedRethrown";
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

            Assert.Catch<Exception>(() => repo.GetAsync(label).Wait());
            mockLogger.Verify(l => l.Error(expectedException, It.IsAny<string>()));

        }

    }
}
