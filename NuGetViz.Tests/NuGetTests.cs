using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGetViz.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NuGet.Protocol.VisualStudio;
using System.Diagnostics;
using NuGet.Configuration;
using Xunit.Abstractions;
using Newtonsoft.Json;
using System.IO;

namespace NuGetViz.Tests
{

    public class NuGetTests
    {
        public NuGetTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        private readonly ITestOutputHelper output;


        [Theory]
        [InlineData("EntityFramework")]
        [InlineData("EntityFramework.Commands")]
        public async Task GetCachedPackageInfo(string PackageID)
        {
            NuGetFactory factory = new NuGetFactory("NuGet", "https://api.nuget.org/v3/index.json");
            var metaResource = await factory.GetUIMetadata();
            var searchResource = await factory.GetSearch();

            var packageMeta = await searchResource.Search(PackageID, new SearchFilter { IncludePrerelease = true }, 0, 100, CancellationToken.None);
            var packagesFound = packageMeta.Where(x => x.Identity.Id.Equals(PackageID, StringComparison.InvariantCultureIgnoreCase)).ToList();

            Assert.False(packagesFound.Count == 0);
            Assert.True(packagesFound.Count > 0);

            var nugetPackage = packagesFound.First();
            var versions = await (await factory.GetMetadata()).GetVersions(PackageID, CancellationToken.None);

            foreach (var version in versions)
            {
                var versionIdentity = new PackageIdentity(nugetPackage.Identity.Id, version);
                var meta = await metaResource.GetMetadata(versionIdentity, CancellationToken.None);
                foreach (var item in meta.DependencySets)
                {
                    Assert.NotNull(item.TargetFramework.Framework);
                }
            }
        }

        [Theory]
        [InlineData("Any")]
        [InlineData(".NETPlatform")]
        [InlineData(".NETFramework")]
        [InlineData(".NETCore")]
        [InlineData("WinRT")]
        [InlineData(".NETMicroFramework")]
        [InlineData(".NETPortable")]
        [InlineData("WindowsPhone")]
        [InlineData("Windows")]
        [InlineData("WindowsPhoneApp")]
        [InlineData("DNX")]
        [InlineData("DNXCore")]
        [InlineData("ASP.NET")]
        [InlineData("ASP.NETCore")]
        [InlineData("Silverlight")]
        [InlineData("native")]
        [InlineData("MonoAndroid")]
        [InlineData("MonoTouch")]
        [InlineData("MonoMac")]
        [InlineData("Xamarin.iOS")]
        [InlineData("Xamarin.Mac")]
        [InlineData("Xamarin.PlayStation3")]
        [InlineData("Xamarin.PlayStation4")]
        [InlineData("Xamarin.PlayStationVita")]
        [InlineData("Xamarin.Xbox360")]
        [InlineData("Xamarin.XboxOne")]
        [InlineData("UAP")]

        public void CreateFrameWorkObject(string framework)
        {
            var fx = new NuGetFramework(framework);
            Assert.NotNull(fx);
            Assert.True(fx.Framework == framework);
            Assert.NotNull(fx.DotNetFrameworkName);

        }

        [Theory]
        [InlineData("EntityFramework", "6.1.3", "Any", false)]
        [InlineData("EntityFramework.Commands", "7.0.0-beta4", ".NETFramework,Version=v4.5.1", true)]
        [InlineData("EntityFramework.Commands", "7.0.0-beta4", "ASP.NET,Version=v5.0", true)]
        [InlineData("EntityFramework.Commands", "7.0.0-beta4", "ASP.NETCore,Version=v5.0", true)]
        [InlineData("EntityFramework.Commands", "7.0.0-beta4", ".NETPortable,Version=v4.6,Profile=Profile151", true)]
        public async Task DependencyTest(string keyword, string version, string framework, bool hasDependencies)
        {
            var factory = new NuGetFactory("NuGet", "https://api.nuget.org/v3/index.json");
            var baseMetaResource = await factory.GetMetadata();
            var metaResource = await factory.GetUIMetadata();
            var depResource = await factory.GetDependency();
            var searchResource = await factory.GetSearch();

            var versionNG = new NuGetVersion(version);
            var fx = NuGetFramework.Parse(framework);
            var packageId = new PackageIdentity(keyword, versionNG);

            Assert.NotNull(fx);
            Assert.NotNull(versionNG);

            // Search with keyword
            var filter = new SearchFilter { IncludePrerelease = true };
            var packagesFound = await searchResource.Search(keyword, filter, 0, 10, CancellationToken.None);
            foreach (var package in packagesFound)
            {
                Assert.NotNull(package.Identity.Id);
                Assert.NotNull(package.Identity.Version);
                Debug.WriteLine(package.Identity.ToString());
                Debug.WriteLine(package.Identity.Version.ToNormalizedString());
            }

            // Get metadata for packageId
            var metaResult = await metaResource.GetMetadata(packageId.Id, true, false, CancellationToken.None);
            var versions = await (await factory.GetMetadata()).GetVersions(packageId.Id, CancellationToken.None);

            foreach (var uiPackageMetadata in metaResult)
            {
                foreach (var dset in uiPackageMetadata.DependencySets)
                {
                    Assert.NotNull(dset.TargetFramework.DotNetFrameworkName);
                    foreach (var depends in dset.Packages)
                    {
                        Debug.WriteLine(depends.ToString());
                        Assert.NotNull(depends.ToString());
                    }
                }
            }

            foreach (var versionF in versions)
            {
                Assert.NotNull(versionF.ToNormalizedString());
            }


            // Get dependency for packageId + version + fx
            var packageDependencyInfo = await depResource.ResolvePackage(packageId, fx, CancellationToken.None);
            foreach (var dependency in packageDependencyInfo.Dependencies)
            {
                var allVersions = await baseMetaResource.GetVersions(dependency.Id, CancellationToken.None);
                var bestMatch = dependency.VersionRange.FindBestMatch(allVersions);

                Assert.NotNull(bestMatch);
            }
        }

        [Theory]
        [InlineData("EntityFramework", "6.1.3", new[] { ".NETFramework,Version=v4.0", ".NETFramework,Version=v4.5" })]
        [InlineData("EntityFramework.Commands", "7.0.0-beta5", new[] { ".NETCore,Version=v5.0", ".NETFramework,Version=v4.5", "DNX,Version=v4.5.1", "DNXCore,Version=v5.0" })]
        [InlineData("Quartz", "2.3.3", new[] { ".NETFramework,Version=v3.5", ".NETFramework,Version=v3.5,Profile=Client", ".NETFramework,Version=v4.0", ".NETFramework,Version=v4.0,Profile=Client" })]
        [InlineData("PlatformSpecific.Analyzer", "1.0.1", new[] { "UAP,Version=v0.0" })]
        public async Task DownloadPackage(string packageID, string version, string[] targetFrameworkSets)
        {
            Settings settings = new Settings(Environment.CurrentDirectory, "NuGet.config");
            var sources = settings.GetSettingValues("activePackageSource");
            output.WriteLine("Sources :" + string.Join(",", sources.Select(x => x.Value)));
            Assert.True(sources.Count > 0);


            var repo = Repository.Factory.GetVisualStudio("http://api.nuget.org/v3/index.json");
            var downloadResource = await repo.GetResourceAsync<DownloadResource>();
            var package = new PackageIdentity(packageID, NuGetVersion.Parse(version));
            var info = await downloadResource.GetDownloadResourceResultAsync(package, settings, CancellationToken.None);

            // RefItems provides us only those frameworks for which there is a folder in lib\...
            // so safer to use that
            var refItems = info.PackageReader.GetReferenceItems().ToList();
            foreach (var item in refItems)
            {
                output.WriteLine("Framework JSON: " + JsonConvert.SerializeObject(item.TargetFramework, Formatting.Indented));
            }

            var fx = info.PackageReader.GetSupportedFrameworks().ToList();
            foreach (var item in fx)
            {
                output.WriteLine("Framework JSON: " + JsonConvert.SerializeObject(item, Formatting.Indented));
            }

            foreach (var item in refItems)
            {
                Assert.True(targetFrameworkSets.Any(x => item.TargetFramework.DotNetFrameworkName.Equals(x)));
            }

            //var packageName = ((System.IO.FileStream)depInfo.PackageStream).Name;
            //Directory.Delete(Path.GetDirectoryName(packageName), true);

            Assert.NotNull(refItems.Count == targetFrameworkSets.Length);
        }
    }
}
