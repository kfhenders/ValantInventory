using System;

namespace KFH.ValantInventory.API.Models
{
    public class Inventory
    {
        public string Label { get; set; }
        public DateTime Expiration { get; set; }
        public string Type { get; set; }
    }
}