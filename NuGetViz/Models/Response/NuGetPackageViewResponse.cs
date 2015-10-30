using NuGetViz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class NuGetPackageViewResponse
    {
        public NuGetPackageViewSearchRequest Request { get; set; }
        public List<NuGetPackageVersion> Versions { get; set; }
        public NuGetPackageVersion LatestVersion { get; internal set; }

        public NuGetPackageViewResponse()
        {
            Versions = new List<NuGetPackageVersion>();
            Request = new NuGetPackageViewSearchRequest();
        }
    }
}