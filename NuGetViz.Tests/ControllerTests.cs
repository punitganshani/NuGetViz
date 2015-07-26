using Newtonsoft.Json;
using NuGet.Frameworks;
using NuGetViz.Controllers;
using NuGetViz.Core;
using NuGetViz.Models;
using NuGetViz.Tests.Fixture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;

namespace NuGetViz.Tests
{

    public class ControllerTests : IClassFixture<ConfigurationFixture>
    {
        public void SetFixture(ConfigurationFixture config)
        {

        }

        [Theory]
        [InlineData("EntityFramework", 100)]
        [InlineData("EntityFramework.Commands", 1)]
   
        [InlineData("di.hook", 1)]
        public async Task SearchTerm(string searchTerm, int maxCount)
        {
            NuGetController controller = new NuGetController();
            var result = await controller.Search(new SearchRequest { SearchTerm = searchTerm, Source = "nuget" }) as ViewResult;
            var model = result.Model as NuGetPackageSearchResponse;
            Assert.True(model.Results.Count <= maxCount);
        }

        [Theory]
        [InlineData("EntityFramework.Commands", 5)]
        [InlineData("di.hook", 1)]
        [InlineData("PropertyChanged.Fody", 5)]
        [InlineData("PCLStorage",5)]
        [InlineData("PlatformSpecific.Analyzer", 3)]
        public async Task ViewPackage(string packageId, int minCount)
        {
            NuGetController controller = new NuGetController();
            var result = await controller.View(new ViewRequest { PackageID = packageId, Source = "nuget" }) as ViewResult;
            var model = result.Model as NuGetPackageViewResponse;
            Assert.True(model.Versions.Count >= minCount);
        }

        [Theory]
        [InlineData("EntityFramework.Commands", "7.0.0-beta5", ".NETPortable", "4.6", "Profile151", false)]
        [InlineData("EntityFramework.Commands", "7.0.0-beta5", "ASP.NETCore", "5.0", null, true)]
        [InlineData("EntityFramework.Commands", "7.0.0-beta5", "ASP.NET", "5.0", null, true)]
        [InlineData("EntityFramework.Commands", "7.0.0-beta5", ".NETFramework", "4.5.1", null, true)]
        [InlineData("EntityFramework", "6.1.3", ".NETFramework", "4.5.1", null, false)]
        [InlineData("di.Hook", "2.1.0", ".NETFramework", "4.5.2", null, false)]
        [InlineData("Microsoft.AspNet.Identity.EntityFramework", "3.0.0-beta5", "DNXCore", "5.0.0.0", null, true)]

        public async Task GetDependency(string packageId, string version, string framework, string fxversion, string profile, bool hasDependencies)
        {
            NuGetController controller = new NuGetController();
            var result = await controller.GetDependencies(packageId, version, "nuget", framework, fxversion, profile, 2) as JsonResult;
            D3DependencyChild ob = result.Data as D3DependencyChild;
            if (hasDependencies)
            {
                Assert.NotNull(ob.children);
            }
            else
            {
                Assert.NotNull(ob);
            }

        }
    }
}
