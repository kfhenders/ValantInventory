using KFH.ValantInventory.Common.Models;
using System.Threading.Tasks;

namespace KFH.ValantInventory.Common.Interfaces
{
    public interface IInventoryDataAccessClient
    {

        /// <summary>
        /// Creates a record of the item in the data store
        /// </summary>
        /// <param name="item">
        /// The item to Add
        /// </param>
        /// <returns>
        /// true if the record was created successfully
        /// </returns>
        Task<bool> CreateAsync(Inventory item);

        /// <summary>
        /// Retrieves the item from the data store
        /// </summary>
        /// <param name="label">
        /// The label of the item to retrieve
        /// </param>
        /// <returns>
        /// The item
        /// </returns>
        Task<Inventory> ReadAsync(string label);

        /// <summary>
        /// Updates an Inventory item in the data store
        /// </summary>
        /// <param name="item">
        /// The item to update
        /// </param>
        /// <returns>
        /// true if the record was updated successfully
        /// </returns>
        Task<bool> UpdateAsync(Inventory item);

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

    }
}
