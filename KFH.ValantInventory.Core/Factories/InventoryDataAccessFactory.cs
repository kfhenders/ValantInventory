using System;
using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.DataAccess;
using NLog;

namespace KFH.ValantInventory.Core.Factories
{
    public class InventoryDataAccessFactory : IInventoryDataAccessFactory
    {
        public ILogger _logger;

        public InventoryDataAccessFactory(ILogger logger)
        {
            _logger = logger;
        }


        public IDeletedInventoryQueue CreateDeletedInventoryQueueClient()
        {
            return new DeletedInventoryQueue(_logger);
        }

        public IExpiredInventoryQueue CreateExpiredInventoryQueueClient()
        {
            return new ExpiredInventoryQueue(_logger);
        }

        public IInventoryDataAccessClient CreateInventoryDataAcessClient()
        {
            return new InventoryDataAccessClient(_logger);
        }
    }
}
