using Newtonsoft.Json;
using NuGetViz.Core;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NuGetViz
{
    public class SharedInfo
    {
        private static SharedInfo _instance;

        public AppConfig Config;

        public static SharedInfo Instance
        {
            get
            {
                return _instance = (_instance ?? new SharedInfo());
            }
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var jsonConfig = File.ReadAllText(Server.MapPath(@"~/nugetviz.json"));
            SharedInfo.Instance.Config = JsonConvert.DeserializeObject<AppConfig>(jsonConfig);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
