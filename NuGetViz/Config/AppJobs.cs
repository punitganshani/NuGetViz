using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Core
{
    public class AppJobs
    {
        [JsonProperty("type")]
        public string JobType { get; set; }

        [JsonProperty("cron")]
        public string CRON { get; set; }
    }
}