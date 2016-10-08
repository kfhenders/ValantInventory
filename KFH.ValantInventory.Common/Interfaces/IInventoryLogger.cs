using System;
using System.ComponentModel;

namespace KFH.ValantInventory.Common.Interfaces
{
    public interface IInventoryLogger
    {
        void Error(Exception exception, string message);
    }
}
