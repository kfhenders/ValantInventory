using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using KFH.ValantInventory.API.Controllers;
using KFH.ValantInventory.Common.Interfaces;
using Moq;
using NUnit.Framework;

namespace KFH.ValantInventory.UnitTests.ApiTests
{
    [TestFixture]
    public partial class InventoryControllerTests
    {

        [Test]
        public async Task InventoryController_Get_OK_Test()
        {
            var label = "InventoryController_Get_OK_Test";
            var commonItem = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(3020, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "dfwfsfs"
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.GetAsync(label)).Returns(Task.FromResult(commonItem));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Get(label).ConfigureAwait(false);
            var apiItem = ReadResponseMessage(response);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.IsInstanceOf<API.Models.Inventory>(apiItem);
            Assert.IsTrue(Utility.AreSame(commonItem,apiItem));

        }

        [Test]
        public async Task InventoryController_Get_NotFound_Test()
        {
            var label = "InventoryController_Get_NotFound_Test";

            var stubLogger = new Mock<IInventoryLogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.GetAsync(label)).Returns(Task.FromResult(default(Common.Models.Inventory)));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Get(label).ConfigureAwait(false);
            var apiItem = ReadResponseMessage(response);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
            Assert.IsNull(apiItem);

        }

        [Test]
        public async Task InventoryController_Get_Expired_NotFound_Test()
        {
            var label = "InventoryController_Get_Expired_NotFound_Test";
            var commonItem = new Common.Models.Inventory
            {
                Label = label,
                ExpirationDateUtc = new DateTime(1900, 2, 15, 11, 32, 12, DateTimeKind.Utc),
                Type = "oijlmljpj"
            };

            var stubLogger = new Mock<IInventoryLogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.GetAsync(label)).Returns(Task.FromResult(commonItem));

            var controller = CreateInventoryController(mockRepository.Object, stubLogger.Object);
            var response = await controller.Get(label).ConfigureAwait(false);
            var apiItem = ReadResponseMessage(response);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
            Assert.IsNull(apiItem);

        }

        [Test]
        public async Task InventoryController_Get_InternalServerError_Test()
        {
            var label = "InventoryController_Get_InternalServerError_Test";

            var mockLogger = new Mock<IInventoryLogger>();
            var mockRepository = new Mock<IInventoryRepository>();
            mockRepository.Setup(r => r.GetAsync(label)).Throws(new Exception("Test Exception Thrown"));

            var controller = CreateInventoryController(mockRepository.Object, mockLogger.Object);
            var response = await controller.Get(label).ConfigureAwait(false);
            var apiItem = ReadResponseMessage(response);

            mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>()));
            Assert.AreEqual(response.StatusCode, HttpStatusCode.InternalServerError);
            Assert.IsNull(apiItem);

        }

        private API.Models.Inventory ReadResponseMessage(HttpResponseMessage response)
        {

            API.Models.Inventory item;
            if (!response.TryGetContentValue(out item))
            {
                return null;
            }
            return item;
        }


        private InventoryController CreateInventoryController(IInventoryRepository repository, IInventoryLogger logger)
        {
            InventoryController controller = new InventoryController(repository, logger);
            controller.Request = new HttpRequestMessage(HttpMethod.Head, "Http://InventoryControllerTests/");
            controller.Configuration = new HttpConfiguration();

            return controller;
        }
    }
}
