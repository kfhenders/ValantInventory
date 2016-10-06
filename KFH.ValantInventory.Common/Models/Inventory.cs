using System;

namespace KFH.ValantInventory.Common.Models
{
    public class Inventory
    {

        public string Label { get; set; }
        public DateTime ExpirationDateUtc { get; set; }
        public string Type { get; set; }

        public bool IsExpired
        {
            get
            {
                return ExpirationDateUtc <= DateTime.UtcNow;
            }
        }
    }
}