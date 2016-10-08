using KFH.ValantInventory.Common.Models;
using System.Threading.Tasks;

namespace KFH.ValantInventory.Common.Interfaces
{
    public interface IInventoryRepository
    {
        /// <summary>
        /// Retrieves the item from the data store
        /// </summary>
        /// <param name="label">
        /// The label of the item to retrieve
        /// </param>
        /// <returns>
        /// The item
        /// </returns>
        Task<Inventory> GetAsync(string label);

        /// <summary>
        /// Adds the item to the data store
        /// </summary>
        /// <param name="item">
        /// The item to Add
        /// </param>
        /// <returns>
        /// true if the record was created successfully
        /// </returns>
        Task<bool> AddAsync(Inventory item);

        /// <summary>
        /// Deletes an item from the data store
        /// </summary>
        /// <param name="label">
        /// The label of the item to delete
        /// </param>
        /// <returns>
        /// true if the record was deleted successfully
        /// </returns>
        Task<bool> DeleteAsync(string label);
        
        /// <summary>
        /// Adds or Updates an Inventory item in the data store
        /// </summary>
        /// <param name="item">
        /// The item to update
        /// </param>
        /// <returns>
        /// true if the item was added, false if the item was updated
        /// </returns>
        Task<bool> AddOrUpdateAsync(Inventory item);

        /// <summary>
        /// Adds an Inventory item to the ExpiredInventory queue. 
        /// If the Expiration date is in the future, it is first set to the current UTC time
        /// </summary>
        /// <param name="label">
        /// The label of the item to expire
        /// </param>/// <returns>
        /// true if the record was expired successfully
        /// </returns>
        Task<bool> ExpireAsync(string label);
    }
}
