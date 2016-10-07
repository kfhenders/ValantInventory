using System;
using System.Net;
using System.Threading.Tasks;
using KFH.ValantInventory.Common.Interfaces;
using Moq;
using NUnit.Framework;
using NLog;

namespace KFH.ValantInventory.UnitTests.ApiTests
{
    public partial class InventoryControllerTests
    {

        [Test]
        public async Task InventoryController_Put_Created_Test()
        {
            var label = "InventoryController_Put_Created_Test";
            var apiItem = new API.Models.Inventory
            {
                Label = label,
                Expiration = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<ILogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.AddOrUpdateAsync(It.IsAny<Common.Models.Inventory>())).Returns(Task.FromResult(true));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Put(apiItem).ConfigureAwait(false);
            
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert .AreEqual($"/api/Inventory/{label}", response.Headers.Location.AbsolutePath);
        }

        [Test]
        public async Task InventoryController_Put_LabelExists_NoContent_Test()
        {
            var label = "InventoryController_Put_LabelExists_NoContent_Test";
            var apiItem = new API.Models.Inventory
            {
                Label = label,
                Expiration = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<ILogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.AddOrUpdateAsync(It.IsAny<Common.Models.Inventory>())).Returns(Task.FromResult(false));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Put(apiItem).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.AreEqual($"/api/Inventory/{label}", response.Headers.Location.AbsolutePath);

        }

        [Test]
        public async Task InventoryController_Put_InternalServerError_Test()
        {
            var label = "InventoryController_Post_InternalServerError_Test";
            var apiItem = new API.Models.Inventory
            {
                Label = label,
                Expiration = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var mockLogger = new Mock<ILogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.AddOrUpdateAsync(It.IsAny<Common.Models.Inventory>())).Throws(new Exception("Test Exception Thrown"));

            var controller = CreateInventoryController(mockRepository.Object, mockLogger.Object);
            var response = await controller.Put(apiItem).ConfigureAwait(false);

            mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>()));
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);

        }
    }
}
