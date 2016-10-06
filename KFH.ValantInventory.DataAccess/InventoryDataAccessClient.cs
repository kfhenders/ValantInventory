using KFH.ValantInventory.Common.Exceptions;
using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Common.Models;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KFH.ValantInventory.DataAccess
{
    public class InventoryDataAccessClient : IInventoryDataAccessClient
    {
        static ConcurrentDictionary<string, Inventory> _localDataStore = new ConcurrentDictionary<string, Inventory>();

        public ILogger _logger;

        public InventoryDataAccessClient(ILogger logger)
        {
            _logger = logger;
        }

        public Task<bool> CreateAsync(Inventory item)
        {
            return Task.FromResult(_localDataStore.TryAdd(item.Label, item));
        }

        public Task<Inventory> ReadAsync(string label)
        {
            if (!_localDataStore.ContainsKey(label))
            {
                throw new InventoryNotFoundException(label);   
            }
            return Task.FromResult(_localDataStore[label]);
        }

        public async Task<bool> UpdateAsync(Inventory item)
        {
            bool added = await CreateAsync(item).ConfigureAwait(false);
            if (!added)
            {
                _localDataStore[item.Label] = item;
            }
            return added;
        }

        public Task<bool> DeleteAsync(string label)
        {
            Inventory item;
            return Task.FromResult(_localDataStore.TryRemove(label, out item));
        }

    }
}
