using KFH.ValantInventory.Common.Models;
using System.Threading.Tasks;

namespace KFH.ValantInventory.Common.Interfaces
{
    public interface IExpiredInventoryQueue
    {
        Task Enqueue(Inventory item);
    }
}
