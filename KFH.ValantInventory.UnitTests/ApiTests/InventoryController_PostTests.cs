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
        public async Task InventoryController_Post_Created_Test()
        {
            var label = "InventoryController_Post_Created_Test";
            var apiItem = new API.Models.Inventory
            {
                Label = label,
                Expiration = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<ILogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.AddAsync(It.IsAny<Common.Models.Inventory>())).Returns(Task.FromResult(true));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Post(apiItem).ConfigureAwait(false);
            
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert .AreEqual($"/api/Inventory/{label}", response.Headers.Location.AbsolutePath);
        }

        [Test]
        public async Task InventoryController_Post_LabelExists_BadRequest_Test()
        {
            var label = "InventoryController_Post_LabelExists_BadRequest_Test";
            var apiItem = new API.Models.Inventory
            {
                Label = label,
                Expiration = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<ILogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.AddAsync(It.IsAny<Common.Models.Inventory>())).Returns(Task.FromResult(false));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Post(apiItem).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task InventoryController_Post_InternalServerError_Test()
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
            mockRepository.Setup(r => r.AddAsync(It.IsAny<Common.Models.Inventory>())).Throws(new Exception("Test Exception Thrown"));

            var controller = CreateInventoryController(mockRepository.Object, mockLogger.Object);
            var response = await controller.Post(apiItem).ConfigureAwait(false);

            mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>()));
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);

        }
    }
}
