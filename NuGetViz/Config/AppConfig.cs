using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Core
{
    public class AppConfig
    {
        [JsonProperty("Repositories")]
        public List<NuGetRepo> Repositories { get; set; }

        [JsonProperty("PackageDownloadFolder")]
        public string PackageDownloadFolderName { get; set; }

        [JsonProperty("Jobs")]
        public List<AppJobs> Jobs { get; set; }

        public AppConfig()
        {
            Repositories = new List<NuGetRepo>();
           // PackageDownloadFolderName = @"c:\temp\.nuget";
            Jobs = new List<AppJobs>();
        }

    }
}