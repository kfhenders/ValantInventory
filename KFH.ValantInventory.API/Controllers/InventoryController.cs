using KFH.ValantInventory.API.Authorization;
using KFH.ValantInventory.API.Models;
using KFH.ValantInventory.Common.Interfaces;
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
        private readonly IInventoryLogger _logger = null;
        private readonly IInventoryRepository _inventoryRepository = null;

        public InventoryController(IInventoryRepository inventoryRepository, IInventoryLogger logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> Get(string label)
        {
            HttpResponseMessage response = null;
            try
            {
                var item = await _inventoryRepository.GetAsync(label).ConfigureAwait(true);
                if (item == null || item.IsExpired)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new Inventory
                    {
                        Label = item.Label,
                        Expiration = item.ExpirationDateUtc,
                        Type = item.Type
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in  KFH.ValantInventory.API.Controllers.Get(string label)");
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return response;
        }

        public async Task<HttpResponseMessage> Post([FromBody]Inventory item)
        {
            HttpResponseMessage response = null;

            try
            {
                bool created = await _inventoryRepository.AddAsync((Common.Models.Inventory)item).ConfigureAwait(true);
                if (created)
                {
                    response = Request.CreateResponse(HttpStatusCode.Created);
                    // Build the location manually rather than using Url.Link() so it can be unit tested
                    response.Headers.Location = new Uri($"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/api/Inventory/{item.Label}");
                }
                else
                {
                    // Assume it wasn't added as it either didn't pass validation or already exists
                    response = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in  KFH.ValantInventory.API.Controllers.Post(Inventory item)");
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return response;
        }

        public async Task<HttpResponseMessage> Put([FromBody]Inventory item)
        {
            HttpResponseMessage response = null;

            try
            {
                bool created = await _inventoryRepository.AddOrUpdateAsync((Common.Models.Inventory)item).ConfigureAwait(true);
                if (created)
                {
                    response = Request.CreateResponse(HttpStatusCode.Created);
                }
                else
                {
                    // Must have been updated
                    response = Request.CreateResponse(HttpStatusCode.NoContent);
                }
                // Build the location manually rather than using Url.Link() so it can be unit tested
                response.Headers.Location = new Uri($"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/api/Inventory/{item.Label}");

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in  KFH.ValantInventory.API.Controllers.Put(Inventory item)");
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return response;
        }

        public async Task<HttpResponseMessage> Delete(string label)
        {
            HttpResponseMessage response = null;

            try
            {
                bool deleted = await _inventoryRepository.DeleteAsync(label).ConfigureAwait(true);
                if (deleted)
                {
                    response = Request.CreateResponse(HttpStatusCode.NoContent);
                }
                else
                {
                    // Assume it wasn't deleted because it didn't exist
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in  KFH.ValantInventory.API.Controllers.Delete(string label)");
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return response;
        }

        [HttpPut]
        [Route("api/Inventory/Expire/{label}")]
        public async Task<HttpResponseMessage> Expire(string label)
        {
            HttpResponseMessage response = null;

            try
            {
                bool expired = await _inventoryRepository.ExpireAsync(label).ConfigureAwait(true);
                if (expired)
                {
                    response = Request.CreateResponse(HttpStatusCode.NoContent);
                }
                else
                {
                    // Assume it wasn't expired because it didn't exist
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception caught in  KFH.ValantInventory.API.Controllers.Expire(string label)");
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return response;
        }
    }
}