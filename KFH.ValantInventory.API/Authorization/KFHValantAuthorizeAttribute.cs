using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace KFH.ValantInventory.API.Authorization
{
    /// <summary>
    /// Class used to authorie access 
    /// </summary>
    public class KFHValantAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}