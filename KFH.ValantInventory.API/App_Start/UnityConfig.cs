using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Core.Factories;
using KFH.ValantInventory.Core.Repositories;
using Microsoft.Practices.Unity;
using NLog;
using System.Web.Http;
using KFH.ValantInventory.Core.Logging;
using Unity.WebApi;

namespace KFH.ValantInventory.API
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<IInventoryLogger, InventoryLogger>();
            container.RegisterType<IInventoryRepository, InventoryRepository>();
            container.RegisterType<IInventoryDataAccessFactory, InventoryDataAccessFactory>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}