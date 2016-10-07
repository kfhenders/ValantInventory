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
        public async Task InventoryController_Expire_NoContent_Test()
        {
            var label = "InventoryController_Expire_NoContent_Test";

            var stubLogger = new Mock<ILogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.ExpireAsync(label)).Returns(Task.FromResult(true));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Expire(label).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

        }

        [Test]
        public async Task InventoryController_Expire_NotFound_Test()
        {
            var label = "InventoryController_Expire_NotFound_Test";

            var stubLogger = new Mock<ILogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.ExpireAsync(label)).Returns(Task.FromResult(false));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Expire(label).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task InventoryController_Expire_InternalServerError_Test()
        {
            var label = "InventoryController_Expire_InternalServerError_Test";

            var mockLogger = new Mock<ILogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.ExpireAsync(label)).Throws(new Exception("Test Exception Thrown"));

            var controller = CreateInventoryController(mockRepository.Object, mockLogger.Object);
            var response = await controller.Expire(label).ConfigureAwait(false);

            mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>()));
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);

        }
    }
}
