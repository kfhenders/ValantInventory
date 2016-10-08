using KFH.ValantInventory.Common.Models;
using System.Threading.Tasks;

namespace KFH.ValantInventory.Common.Interfaces
{
    public interface IDeletedInventoryQueue
    {
        Task Enqueue(Inventory item);
    }
}
