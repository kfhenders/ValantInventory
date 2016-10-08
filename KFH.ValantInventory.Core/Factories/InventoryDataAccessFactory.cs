using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.DataAccess;

namespace KFH.ValantInventory.Core.Factories
{
    public class InventoryDataAccessFactory : IInventoryDataAccessFactory
    {
        private readonly IInventoryLogger _logger;

        public InventoryDataAccessFactory(IInventoryLogger logger)
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
