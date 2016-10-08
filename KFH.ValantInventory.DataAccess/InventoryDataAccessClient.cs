using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Common.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KFH.ValantInventory.DataAccess
{
    public class InventoryDataAccessClient : IInventoryDataAccessClient
    {
        private static readonly  ConcurrentDictionary<string, Inventory> LocalDataStore = new ConcurrentDictionary<string, Inventory>();

        private readonly IInventoryLogger _logger;

        public InventoryDataAccessClient(IInventoryLogger logger)
        {
            _logger = logger;
        }

        public Task<bool> CreateAsync(Inventory item)
        {
            return Task.FromResult(LocalDataStore.TryAdd(item.Label, item));
        }

        public Task<Inventory> ReadAsync(string label)
        {
            if (!LocalDataStore.ContainsKey(label))
            {
                return Task.FromResult(default(Inventory));
            }
            return Task.FromResult(LocalDataStore[label]);
        }

        public Task<bool> UpdateAsync(Inventory item)
        {
            if (!LocalDataStore.ContainsKey(item.Label))
            {
                return Task.FromResult(false);
            }

            LocalDataStore[item.Label] = item;

            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(string label)
        {
            Inventory item;
            return Task.FromResult(LocalDataStore.TryRemove(label, out item));
        }

    }
}
