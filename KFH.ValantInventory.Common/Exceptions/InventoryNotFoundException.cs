using System;

namespace KFH.ValantInventory.Common.Exceptions
{
    /// <summary>
    /// Exception to throw when an item is not found
    /// </summary>
    public class InventoryNotFoundException : Exception
    {
        public InventoryNotFoundException(string label) : base($"Unable to locate item with Label: {label}") { }
    }
}
