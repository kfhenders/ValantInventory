using KFH.ValantInventory.Common.Interfaces;
using KFH.ValantInventory.Core.Factories;
using KFH.ValantInventory.Core.Repositories;
using Microsoft.Practices.Unity;
using NLog;
using System.Web.Http;
using Unity.WebApi;

namespace KFH.ValantInventory.API
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<ILogger>(new InjectionFactory(f => LogManager.GetCurrentClassLogger(typeof(Logger))));
            container.RegisterType<IInventoryRepository, InventoryRepository>();
            container.RegisterType<IInventoryDataAccessFactory, InventoryDataAccessFactory>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}