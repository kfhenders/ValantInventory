using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Common.Models;
using NLog;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KFH.ValantInventory.DataAccess
{

    public class ExpiredInventoryQueue : IExpiredInventoryQueue
    {
        static ConcurrentQueue<Inventory> _localQueue = new ConcurrentQueue<Inventory>();

        public ILogger _logger;

        public ExpiredInventoryQueue(ILogger logger)
        {
            _logger = logger;
        }

        public Task Enqueue(Inventory item)
        {
            _localQueue.Enqueue(item);
            return Task.CompletedTask;
        }
    }
}
