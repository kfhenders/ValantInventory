namespace KFH.ValantInventory.Common.Interfaces
{
    /// <summary>
    /// Class used for creating clients required to access Inventory Data stores
    /// </summary>
    public interface IInventoryDataAccessFactory
    {
        IInventoryDataAccessClient CreateInventoryDataAcessClient();
        IDeletedInventoryQueue CreateDeletedInventoryQueueClient();
        IExpiredInventoryQueue CreateExpiredInventoryQueueClient();
    }
}
