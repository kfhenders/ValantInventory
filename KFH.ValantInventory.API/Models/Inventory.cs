using System;

namespace KFH.ValantInventory.API.Models
{
    public class Inventory
    {
        public string Label { get; set; }
        public DateTime Expiration { get; set; }
        public string Type { get; set; }

        public static explicit operator Inventory(Common.Models.Inventory commonItem)
        {
            return new Inventory
            {
                Label = commonItem.Label,
                Expiration = commonItem.ExpirationDateUtc,
                Type = commonItem.Type
            };
        }

        public static explicit operator Common.Models.Inventory(Inventory apiItem)
        {
            return new Common.Models.Inventory
            {
                Label = apiItem.Label,
                ExpirationDateUtc = apiItem.Expiration,
                Type = apiItem.Type
            };
        }
    }
}