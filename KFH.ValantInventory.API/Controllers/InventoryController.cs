using KFH.ValantInventory.API.Authorization;
using KFH.ValantInventory.API.Models;
using KFH.ValantInventory.Common.Interfaces;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace KFH.ValantInventory.API.Controllers
{
    [KFHValantAuthorize]
    public class InventoryController : ApiController
    {
        private ILogger _logger = null;
        private IInventoryRepository _inventoryRepository = null;
        
        public InventoryController(IInventoryRepository inventoryRepository, ILogger logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> Get(string label)
        {
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }

        public async Task<HttpResponseMessage> Post([FromBody]Inventory item )
        {
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }

        public async Task<HttpResponseMessage> Put([FromBody]string value)
        {
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }

        public async Task<HttpResponseMessage> Delete(string label)
        {
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }

        [HttpPut]
        [Route("api/Inventory/Expire/{label}")]
        public async Task<HttpResponseMessage> Expire(string label)
        {
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }
    }
}
