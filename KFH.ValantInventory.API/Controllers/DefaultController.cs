using System.Collections.Generic;
using System.Web.Http;

namespace KFH.ValantInventory.API.Controllers
{
    public class DefaultController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new List<string> { "A", "b", "C" };
        }
    }
}
