using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class NuGetPackageHierarchyResponse
    {
        public string PackageID { get; set; }
        public string Version { get; set; }
        public string Source { get; set; }

        public bool IsPreRelease { get; set; }

        public NuGetFrameworkInfo FrameworkInfo { get; set; }
      
        public NuGetPackageVersion PackageInfo { get; internal set; }  
    }
}