using System;
using NUnit.Framework;

namespace KFH.ValantInventory.UnitTests.ApiTests
{
    [TestFixture]
    public class InventoryModelTests
    {
        [Test]
        public void Inventory_Explicit_CommonToApi_Test()
        {
            Common.Models.Inventory commonItem = new Common.Models.Inventory
            {
                Label = "Inventory_Explicit_CommonToApi_Test",
                ExpirationDateUtc = new DateTime(1998, 1, 12, 4, 50, 6, DateTimeKind.Utc),
                Type = "sdefg",
                ExpirationQueued = true
            };

            var apiItem = (API.Models.Inventory) commonItem;
            Assert.IsTrue(Utility.AreSame(commonItem, apiItem));
        }

        [Test]
        public void Inventory_Explicit_ApiToCommon_Test()
        {
            API.Models.Inventory apiItem = new API.Models.Inventory
            {
                Label = "Inventory_Explicit_ApiToCommon_Test",
                Expiration = new DateTime(1998, 1, 12, 4, 50, 6, DateTimeKind.Utc),
                Type = "sdefg"
            };

            var commonItem = (Common.Models.Inventory)apiItem;
            Assert.IsTrue(Utility.AreSame(commonItem, apiItem));
        }
    }
}
