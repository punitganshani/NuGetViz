using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NuGetViz.Startup))]
namespace NuGetViz
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
