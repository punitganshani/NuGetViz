using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class NuGetPackageVersion
    {
        public string PackageID { get; set; }
        public bool IsReleaseVersion { get; set; }
        public string Version { get; internal set; }

        [DisplayFormat(DataFormatString = "{0:MMM, dd yyyy}")]
        public string LastUpdated { get; internal set; }

        public DateTime LastUpdatedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int Downloads { get; internal set; }
        public string OrderKey { get; internal set; }
        public List<NuGetFrameworkInfo> SupportedFrameworks { get; internal set; }       
       
        public bool IsLegacy { get; internal set; }

        public string Description { get; internal set; }

        public string Owners { get; internal set; }
        public string Authors { get; internal set; }

        public string ProjectUri { get; set; }
        public string LicenseUri { get; internal set; }
        public string ReportAbuseUri { get; internal set; }
        public Uri IconUri { get; internal set; }
        public bool IsPreRelease { get; internal set; }

        public NuGetPackageVersion()
        {
            SupportedFrameworks = new List<NuGetFrameworkInfo>();
        }
    }
}

