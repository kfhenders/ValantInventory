using System;
using System.Net;
using System.Threading.Tasks;
using KFH.ValantInventory.Common.Interfaces;
using Moq;
using NUnit.Framework;

namespace KFH.ValantInventory.UnitTests.ApiTests
{
    public partial class InventoryControllerTests
    {

        [Test]
        public async Task InventoryController_Delete_NoContent_Test()
        {
            var label = "InventoryController_Delete_NoContent_Test";

            var stubLogger = new Mock<IInventoryLogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.DeleteAsync(label)).Returns(Task.FromResult(true));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Delete(label).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

        }

        [Test]
        public async Task InventoryController_Delete_NotFound_Test()
        {
            var label = "InventoryController_Delete_NotFound_Test";

            var stubLogger = new Mock<IInventoryLogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.DeleteAsync(label)).Returns(Task.FromResult(false));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Delete(label).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task InventoryController_Delete_InternalServerError_Test()
        {
            var label = "InventoryController_Delete_InternalServerError_Test";

            var mockLogger = new Mock<IInventoryLogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.DeleteAsync(label)).Throws(new Exception("Test Exception Thrown"));

            var controller = CreateInventoryController(mockRepository.Object, mockLogger.Object);
            var response = await controller.Delete(label).ConfigureAwait(false);

            mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>()));
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);

        }
    }
}
