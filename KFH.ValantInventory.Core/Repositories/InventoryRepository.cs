using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Common.Models;
using System;
using System.Threading.Tasks;

namespace KFH.ValantInventory.Core.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IInventoryLogger _logger = null;

        private readonly IInventoryDataAccessFactory _dataAccessFactory;

        Lazy<IInventoryDataAccessClient> LazyDataAccessClient =>
            new Lazy<IInventoryDataAccessClient>(() => _dataAccessFactory.CreateInventoryDataAcessClient());

        Lazy<IDeletedInventoryQueue> LazyDeletedInventoryQueue =>
            new Lazy<IDeletedInventoryQueue>(() => _dataAccessFactory.CreateDeletedInventoryQueueClient());

        Lazy<IExpiredInventoryQueue> LazyExpiredInventoryQueue =>
            new Lazy<IExpiredInventoryQueue>(() => _dataAccessFactory.CreateExpiredInventoryQueueClient());

        public InventoryRepository(IInventoryDataAccessFactory dataAccessFactory, IInventoryLogger logger)
        {
            _logger = logger;
            _dataAccessFactory = dataAccessFactory;
        }

        public async Task<bool> AddAsync(Inventory item)
        {
            bool added = false;
            try
            {
                // Make sure an item with the same label doesn't already exist
                var existingItem = await LazyDataAccessClient.Value.ReadAsync(item.Label).ConfigureAwait(false);
                if (existingItem != null)
                {
                    return false;
                }
                // Convert expiration to Utc if it isn't already
                if (item.ExpirationDateUtc.Kind != DateTimeKind.Utc)
                {
                    item.ExpirationDateUtc = item.ExpirationDateUtc.ToUniversalTime();
                }
                added = await LazyDataAccessClient.Value.CreateAsync(item).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in KFH.ValantInventory.Core.Repositories.AddAsync(Inventory item)");
                throw;
            }

            return added;
        }

        public async Task<bool> AddOrUpdateAsync(Inventory item)
        {
            bool added;
            try
            {
                added = await AddAsync(item).ConfigureAwait(false);
                if (!added)
                {
                    // Convert expiration to Utc if it isn't already
                    if (item.ExpirationDateUtc.Kind != DateTimeKind.Utc)
                    {
                        item.ExpirationDateUtc = item.ExpirationDateUtc.ToUniversalTime();
                    }
                    await LazyDataAccessClient.Value.UpdateAsync(item);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in KFH.ValantInventory.Core.Repositories.AddOrUpdateAsync(Inventory item)");
                throw;
            }

            return added;

        }

        public async Task<bool> DeleteAsync(string label)
        {
            bool deleted = false;
            try
            {
                // Make sure the item exists
                var existingItem = await LazyDataAccessClient.Value.ReadAsync(label).ConfigureAwait(false);
                if (existingItem == null)
                {
                    return false;
                }

                deleted = await LazyDataAccessClient.Value.DeleteAsync(label).ConfigureAwait(false);
                if (deleted)
                {
                    await LazyDeletedInventoryQueue.Value.Enqueue(existingItem).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in KFH.ValantInventory.Core.Repositories.DeleteAsync(string label)");
                throw;
            }

            return deleted;
        }

        public async Task<bool> ExpireAsync(string label)
        {
            bool expired = false;
            try
            {
                // Make sure the item exists
                var existingItem = await LazyDataAccessClient.Value.ReadAsync(label).ConfigureAwait(false);
                if (existingItem == null)
                {
                    return false;
                }
                if (existingItem.ExpirationQueued)
                {
                    return true;
                }

                if (existingItem.ExpirationDateUtc > DateTime.UtcNow)
                {
                    existingItem.ExpirationDateUtc = DateTime.UtcNow.AddSeconds(-1);
                }

                await LazyExpiredInventoryQueue.Value.Enqueue(existingItem).ConfigureAwait(false);
                existingItem.ExpirationQueued = true;

                expired = await LazyDataAccessClient.Value.UpdateAsync(existingItem).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in KFH.ValantInventory.Core.Repositories.ExpireAsync(string label)");
                throw;
            }

            return expired;
        }

        public async Task<Inventory> GetAsync(string label)
        {
            var result = default(Inventory);
            try
            {
                result = await LazyDataAccessClient.Value.ReadAsync(label).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in KFH.ValantInventory.Core.RepositoriesGetAsync(string label)");
                throw;
            }

            return result;
        }
    }
}
