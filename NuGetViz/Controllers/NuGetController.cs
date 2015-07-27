using System;
using System.Web.Mvc;
using NuGetViz.Models;
using NuGetViz.Core;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace NuGetViz.Controllers
{
    [CustomErrorHandle]
    public partial class NuGetController : Controller
    {
        CacheNugetRepository _cache = new CacheNugetRepository();

        [HttpPost]
        public virtual async Task<ActionResult> Search(SearchRequest request)
        {
            Trace.TraceInformation("[NuGet.Search] Package: " + request.SearchTerm + " Source: " + request.Source);
            if (string.IsNullOrEmpty(request.SearchTerm))
                throw new UserActionException("Package Name is required during search", "CON.NUGET.SP1", new ArgumentNullException("PackageID"));

            NuGetPackageSearchResponse model = await _cache.SearchPackages(request.SearchTerm, request.Source);
            return View(model);
        }

        [HttpPost]
        public new virtual async Task<ActionResult> View(ViewRequest request)
        {
            Trace.TraceInformation("[NuGet.View] Package: " + request.PackageID + " Source: " + request.Source);
            if (string.IsNullOrEmpty(request.PackageID))
                throw new UserActionException("Package Name is required during search", "CON.NUGET.VP1", new ArgumentNullException("PackageID"));

            NuGetPackageViewResponse model = await _cache.ViewPackageInfo(request.PackageID, request.Source);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Analyze(AnalyzeRequest request)
        {
            Trace.TraceInformation("[NuGet.Analyze] Package: " + request.PackageID + " request.Source: " + request.Source + " request.Version: " + request.Version + " FX: " + request.FrameworkProfile);
            if (string.IsNullOrEmpty(request.PackageID))
                throw new UserActionException("Package Name is required to analyze", "CON.NUGET.AP1", new ArgumentNullException("request.PackageID"));

            if (string.IsNullOrEmpty(request.Version))
                throw new UserActionException("Package request.Version is required to analyze", "CON.NUGET.AP2", new ArgumentNullException("request.Version"));

            if (string.IsNullOrEmpty(request.Source))
                throw new UserActionException("Nuget request.Source is required to analyze", "CON.NUGET.AP3", new ArgumentNullException("request.Source"));

            if (string.IsNullOrEmpty(request.Framework))
                throw new UserActionException("request.Framework is required to analyze", "CON.NUGET.AP4", new ArgumentNullException("request.Framework"));

            if (string.IsNullOrEmpty(request.FrameworkVersion))
                throw new UserActionException("request.Framework request.Version is required to analyze", "CON.NUGET.AP5", new ArgumentNullException("request.FrameworkVersion"));

            var model = new NuGetPackageHierarchyResponse
            {
                PackageID = request.PackageID,
                Version = request.Version,
                Source = request.Source,
                FrameworkInfo = new NuGetFrameworkInfo
                {
                    Framework = request.Framework,
                    Version = request.FrameworkVersion,
                    Profile = request.FrameworkProfile
                }
            };

            model.PackageInfo = await _cache.ViewPackageVersionInfo(request.PackageID, request.Version, request.Source);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<ActionResult> GetDependencies(string PackageID, string PackageVersion,
            string Source, string Framework, string FrameworkVersion, string FrameworkProfile, int MaxLevel = 3)
        {
            Trace.TraceInformation("[NuGet.GetDependencies] Package: " + PackageID + " Source: " + Source + " Version: " + PackageVersion
                + " FX: " + FrameworkProfile + " MaxLevel:" + MaxLevel);

            if (string.IsNullOrEmpty(PackageID))
                throw new UserActionException("Package Name is required to view dependencies", "CON.NUGET.GD1", new ArgumentNullException("PackageID"));

            if (string.IsNullOrEmpty(PackageVersion))
                throw new UserActionException("Package Version is required to view dependencies", "CON.NUGET.GD2", new ArgumentNullException("PackageVersion"));

            if (string.IsNullOrEmpty(Source))
                throw new UserActionException("Nuget Source is required to view dependencies", "CON.NUGET.GD3", new ArgumentNullException("Source"));

            if (string.IsNullOrEmpty(Framework))
                throw new UserActionException("Framework is required to analyze", "CON.NUGET.GD4", new ArgumentNullException("Framework"));

            if (string.IsNullOrEmpty(FrameworkVersion))
                throw new UserActionException("Framework Version is required to analyze", "CON.NUGET.GD5", new ArgumentNullException("FrameworkVersion"));

            if (MaxLevel > 4)
            {
                MaxLevel = 4;
            }

            D3DependencyChild model = await _cache.ViewDependencies(PackageID, PackageVersion, Source, Framework, FrameworkVersion, FrameworkProfile, MaxLevel);
            return Json(model);
        }

        public virtual async Task<FileResult> Download(string Source, string PackageID, string Version)
        {
            Trace.TraceInformation("[NuGet.GetDependencies] Package: " + PackageID + " Version: " + Version);

            if (string.IsNullOrEmpty(PackageID))
                throw new UserActionException("Package Name is required to view dependencies", "CON.NUGET.DO1", new ArgumentNullException("PackageID"));

            if (string.IsNullOrEmpty(Version))
                throw new UserActionException("Package Version is required to view dependencies", "CON.NUGET.DO2", new ArgumentNullException("PackageVersion"));

            var stream = await _cache.DownloadPackage(Source, PackageID, Version);

            return File(stream, "application/octet-stream", String.Format("{0}.{1}.nupkg", PackageID, Version));
        }
    }
}
