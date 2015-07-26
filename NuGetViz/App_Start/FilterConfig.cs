using NuGetViz.Core;
using System.Web;
using System.Web.Mvc;

namespace NuGetViz
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomErrorHandle());
        }
    }
}
