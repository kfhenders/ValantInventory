using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Common.Models;
using NLog;
using System;
using System.Threading.Tasks;

namespace KFH.ValantInventory.Core.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {

        ILogger _logger = null;

        IInventoryDataAccessFactory _dataAccessFactory;

        Lazy<IInventoryDataAccessClient> _lazyDataAccessClient =>
            new Lazy<IInventoryDataAccessClient>(() => { return _dataAccessFactory.CreateInventoryDataAcessClient(); });

        Lazy<IDeletedInventoryQueue> _lazyDeletedInventoryQueue =>
            new Lazy<IDeletedInventoryQueue>(() => { return _dataAccessFactory.CreateDeletedInventoryQueueClient(); });

        Lazy<IExpiredInventoryQueue> _lazyExpiredInventoryQueue =>
            new Lazy<IExpiredInventoryQueue>(() => { return _dataAccessFactory.CreateExpiredInventoryQueueClient(); });

        public InventoryRepository(IInventoryDataAccessFactory dataAccessFactory, ILogger logger)
        {
            _logger = logger;
            _dataAccessFactory = dataAccessFactory;            
        }

        public async Task<bool> AddAsync(Inventory item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddOrUpdateAsync(Inventory item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string label)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExpireAsync(string label)
        {
            throw new NotImplementedException();
        }

        public Task<Inventory> GetAsync(string label)
        {
            throw new NotImplementedException();
        }
    }
}
