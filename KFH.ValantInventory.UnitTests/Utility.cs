namespace KFH.ValantInventory.UnitTests
{
    public static class Utility
    {
        public static bool AreSame(Common.Models.Inventory commonInventory, API.Models.Inventory apiInventory)
        {
            if (commonInventory == null || apiInventory == null)
            {
                return false;
            }
            return apiInventory.Label == commonInventory.Label
                   && apiInventory.Expiration == commonInventory.ExpirationDateUtc
                   && apiInventory.Type == commonInventory.Type;
        }

        public static bool AreSame(API.Models.Inventory apiInventory, Common.Models.Inventory commonInventory)
        {
            return AreSame(commonInventory, apiInventory);
        }

    }
}
