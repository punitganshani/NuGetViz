using Newtonsoft.Json;
using NuGet.Configuration;
using NuGetViz.Core;
using NuGetViz.Jobs;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NuGetViz
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           
            // Create Configuration
            SharedInfo.Create(Server.MapPath(@"~/"));

            //SharedInfo.Create(JsonConvert.DeserializeObject<AppConfig>(jsonConfig), nugetConfig);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            JobScheduler.Start();
        }
    }
}
