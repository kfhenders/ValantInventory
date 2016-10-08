using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Common.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KFH.ValantInventory.DataAccess
{

    public class ExpiredInventoryQueue : IExpiredInventoryQueue
    {
        private static readonly ConcurrentQueue<Inventory> LocalQueue = new ConcurrentQueue<Inventory>();

        private readonly IInventoryLogger _logger;

        public ExpiredInventoryQueue(IInventoryLogger logger)
        {
            _logger = logger;
        }

        public Task Enqueue(Inventory item)
        {
            LocalQueue.Enqueue(item);
            return Task.CompletedTask;
        }
    }
}
