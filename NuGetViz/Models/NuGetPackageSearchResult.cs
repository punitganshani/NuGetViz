using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class NuGetPackageSearchResult
    {
        public string Title { get; internal set; }
        public string Summary { get; internal set; }
        public int? DownloadCount { get; internal set; }
        public Uri IconUri { get; internal set; }
        public string PackageID { get; internal set; }
        public string LatestVersion { get; internal set; }
        public string Authors { get; internal set; }
        public string Tags { get; internal set; }
    }
}