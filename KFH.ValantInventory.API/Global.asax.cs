using System.Net.Http.Formatting;
using System.Web.Http;

namespace KFH.ValantInventory.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ForceJson();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        private void ForceJson()
        {
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
        }
    }
}

