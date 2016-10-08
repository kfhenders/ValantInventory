using System;
using KFH.ValantInventory.Common.Interfaces;
using NLog;

namespace KFH.ValantInventory.Core.Logging
{
    public class InventoryLogger : IInventoryLogger
    {
        private static readonly ILogger NLogger = LogManager.GetCurrentClassLogger(typeof(Logger));

        public void Error(Exception exception, string message)
        {
            NLogger.Error(exception, message);
        }
    }
}
