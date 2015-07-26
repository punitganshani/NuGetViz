using System;
using System.Collections.Generic;
using System.Linq;
using NuGetViz.Models;
using NuGet;
using System.Threading;
using NuGet.Versioning;
using NuGet.Frameworks;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.VisualStudio;

namespace NuGetViz.Core
{
    public class CacheNugetRepository : InMemoryCacheStore
    {
        public async Task<NuGetPackageSearchResponse> SearchPackages(string searchTerm, string source)
        {
            if (string.IsNullOrEmpty(searchTerm))
                throw new ArgumentNullException("searchTerm");
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            NuGetFactory factory = new NuGetFactory(source);
            var model = new NuGetPackageSearchResponse
            {
                Request = new NuGetPackageSearchRequest
                {
                    SearchKeyword = searchTerm,
                    Source = source,
                    SourceUrl = factory.SourceUrl
                }
            };

            var searchResource = await factory.GetSearch();
            var packageMeta = await searchResource.Search(searchTerm, new SearchFilter { IncludePrerelease = true }, 0, 100, CancellationToken.None);
            var packagesFound = packageMeta.ToList();

            foreach (var searchInfo in packagesFound)
            {
                var result = new NuGetPackageSearchResult();
                result.Title = searchInfo.Title;
                result.Summary = searchInfo.Summary;
                result.DownloadCount = searchInfo.LatestPackageMetadata.DownloadCount;
                result.IconUri = searchInfo.LatestPackageMetadata.IconUrl;
                result.PackageID = searchInfo.Identity.Id;
                result.LatestVersion = searchInfo.LatestPackageMetadata.Identity.Version.ToNormalizedString();
                result.Authors = searchInfo.LatestPackageMetadata.Authors;
                result.Tags = searchInfo.LatestPackageMetadata.Tags;

                model.Results.Add(result);
            }

            return model;
        }

        public async Task<NuGetPackageViewResponse> ViewPackageInfo(string packageID, string source)
        {
            if (string.IsNullOrEmpty(packageID))
                throw new ArgumentNullException("packageID");

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            NuGetFactory factory = new NuGetFactory(source);
            var model = new NuGetPackageViewResponse();
            model.Request.PackageID = packageID;
            model.Request.SourceUrl = factory.SourceUrl;
            model.Request.Source = source;

            var metaResource = await factory.GetUIMetadata();
            var versions = await CacheGetAllVersionsOfPackage(factory, packageID);

            foreach (var version in versions)
            {
                var packageVersion = new NuGetPackageVersion();

                #region Meta
                var versionIdentity = new PackageIdentity(packageID, version);
                packageVersion = await CacheGetVersionMeta(metaResource, versionIdentity, true);
                #endregion

                #region Version Specific
                packageVersion.IsReleaseVersion = !version.IsPrerelease;
                packageVersion.IsLegacy = version.IsLegacyVersion;
                packageVersion.IsPreRelease = version.IsPrerelease;
                packageVersion.Version = version.ToNormalizedString();
                packageVersion.OrderKey = packageVersion.LastUpdatedDate.ToString("yyyyMMdd") + packageVersion.Version;
                packageVersion.LastUpdated = (packageVersion.LastUpdatedDate == DateTime.MinValue)
                    ? string.Empty : packageVersion.LastUpdatedDate.ToString("MMM dd, yyyy");
                #endregion

                model.Versions.Add(packageVersion);
            }

            model.Versions = model.Versions.OrderByDescending(x => x.OrderKey).ToList();
            model.LatestVersion = model.Versions.FirstOrDefault();

            return model;
        }

        public async Task<NuGetPackageVersion> ViewPackageVersionInfo(string packageID, string packageVersion,
                string source)
        {
            if (string.IsNullOrEmpty(packageID))
                throw new ArgumentNullException("packageID");

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            if (string.IsNullOrEmpty(packageVersion))
                throw new ArgumentNullException("packageVersion");

            NuGetFactory factory = new NuGetFactory(source);
            var metaResource = await factory.GetUIMetadata();
            var versionIdentity = new PackageIdentity(packageID, NuGetVersion.Parse(packageVersion));

            return await CacheGetVersionMeta(metaResource, versionIdentity, false);
        }

        public async Task<D3DependencyChild> ViewDependencies(string packageID, string packageVersion, string source,
            string framework, string frameworkVersion, string frameworkProfile, int maxLevel = 3)
        {
            if (string.IsNullOrEmpty(packageID))
                throw new ArgumentNullException("packageID");

            if (string.IsNullOrEmpty(packageVersion))
                throw new ArgumentNullException("packageVersion");

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            if (string.IsNullOrEmpty(framework))
                throw new ArgumentNullException("framework");

            if (string.IsNullOrEmpty(frameworkVersion))
                throw new ArgumentNullException("frameworkVersion");

            var factory = new NuGetFactory(source);
            NuGetFramework fx = new NuGetFramework(framework, Version.Parse(frameworkVersion), frameworkProfile);
            PackageIdentity package = new PackageIdentity(packageID, NuGetVersion.Parse(packageVersion));
            var root = new D3DependencyChild
            {
                key = packageID,
                version = packageVersion,
                Level = 0
            };

            await DoSearch(factory, root, maxLevel, package, fx);

            return root;

        }

        #region Private Methods

        private async Task DoSearch(NuGetFactory factory, D3DependencyChild current,
                                 int maxLevel,
                                 PackageIdentity package, NuGetFramework fx)
        {
            if (current.Level > maxLevel)
            {
                return;
            }
            
            // same package may be traversed multiple times, so we can cache it
            // don't cache the tree as depth may change for the same package
            SourcePackageDependencyInfo packageDependencyInfo = await CacheResolvePackage(factory, package, fx);
            if (packageDependencyInfo.Dependencies.Any())
            {
                if (current.children == null)
                {
                    current.children = new List<D3DependencyChild>();
                }

                foreach (var dependency in packageDependencyInfo.Dependencies)
                {
                    // unlikely that the version of a particular dependency will change in 30 minutes
                    // so we can cache it
                    IEnumerable<NuGetVersion> allVersions = await CacheGetAllVersionsOfDependency(factory, dependency);
                    if (allVersions.Any())
                    {
                        var bestMatch = dependency.VersionRange.FindBestMatch(allVersions);

                        var child = new D3DependencyChild
                        {
                            key = dependency.Id,
                            version = bestMatch.Version.ToString(),
                            parent = current.key,
                            Level = current.Level + 1
                        };

                        current.children.Add(child);

                        var nextPackage = new PackageIdentity(child.key, bestMatch);
                        await DoSearch(factory, child, maxLevel, nextPackage, fx);
                    }
                }
            }
        }


        private static async Task<NuGetPackageVersion> GetVersionMeta(PackageIdentity versionIdentity,
          UIMetadataResource metaResource, bool getDependencies)
        {
            var packageVersion = new NuGetPackageVersion();
            var meta = await metaResource.GetMetadata(versionIdentity, CancellationToken.None);
            packageVersion.PackageID = meta.Identity.Id;
            packageVersion.Description = meta.Description;

            //Uri
            packageVersion.IconUri = meta.IconUrl;
            packageVersion.ProjectUri = meta.ProjectUrl == null ? "#" : meta.ProjectUrl.AbsoluteUri;
            packageVersion.LicenseUri = meta.LicenseUrl == null ? "#" : meta.LicenseUrl.AbsoluteUri;
            packageVersion.ReportAbuseUri = meta.ReportAbuseUrl.AbsoluteUri;

            //TODO:Currently NuGet API return blank
            packageVersion.Authors = String.Join(",", meta.Authors);
            packageVersion.Owners = String.Join(",", meta.Owners);

            //Supported Frameworks
            packageVersion.SupportedFrameworks = new List<NuGetFrameworkInfo>();
            if (getDependencies)
            {
                packageVersion.HasDependencies = meta.DependencySets.Any();
                foreach (var item in meta.DependencySets)
                {
                    NuGetFrameworkInfo fxInfo = NuGetFrameworkInfo.CreateFrom(item.TargetFramework);
                    packageVersion.SupportedFrameworks.Add(fxInfo);
                }
                if (!packageVersion.SupportedFrameworks.Any())
                {
                    packageVersion.SupportedFrameworks.Add(NuGetFrameworkInfo.Any());
                }
            }
            return packageVersion;
        }

        #endregion

        #region Cache Wrappers
        private async Task<NuGetPackageVersion> CacheGetVersionMeta(UIMetadataResource metaResource, PackageIdentity versionIdentity, bool showDependencies)
        {
            string key = String.Format(@"{0}-{1}", versionIdentity.ToString(), showDependencies);
            if (base.IsInCache<NuGetPackageVersion>("GetVersionMeta", key))
            {
                return Get<NuGetPackageVersion>("GetVersionMeta", key);
            }
            else
            {
                var meta = await metaResource.GetMetadata(versionIdentity, CancellationToken.None);
                var output = await GetVersionMeta(versionIdentity, metaResource, showDependencies);
                return Get<NuGetPackageVersion>("GetVersionMeta", key, () => { return output; });
            }
        }

        private async Task<SourcePackageDependencyInfo> CacheResolvePackage(NuGetFactory factory, PackageIdentity package, NuGetFramework fx)
        {
            string key = String.Format(@"{0}-{1}", package.ToString(), fx.ToString());
            if (base.IsInCache<SourcePackageDependencyInfo>("CacheResolvePackage", key))
            {
                return Get<SourcePackageDependencyInfo>("CacheResolvePackage", key);
            }
            else
            {
                var depResource = await factory.GetDependency();
                var output = await depResource.ResolvePackage(package, fx, CancellationToken.None);
                return Get<SourcePackageDependencyInfo>("CacheResolvePackage", key, () => { return output; });
            }
          
        }
        private async Task<IEnumerable<NuGetVersion>> CacheGetAllVersionsOfDependency(NuGetFactory factory,
                NuGet.Packaging.Core.PackageDependency dependency)
        {
            return await CacheGetAllVersionsOfPackage(factory, dependency.Id);
        }

        private async Task<IEnumerable<NuGetVersion>> CacheGetAllVersionsOfPackage(NuGetFactory factory, string packageID)
        {
            if (base.IsInCache<IEnumerable<NuGetVersion>>("CacheGetAllVersionsOfPackage", packageID))
            {
                return Get<IEnumerable<NuGetVersion>>("CacheGetAllVersionsOfPackage", packageID);
            }
            else
            {
                var baseMetaResource = await factory.GetMetadata();
                var output = await baseMetaResource.GetVersions(packageID, CancellationToken.None);
                return Get<IEnumerable<NuGetVersion>>("CacheGetAllVersionsOfPackage", packageID, () => { return output; });
            }
        }

        #endregion
    }
}