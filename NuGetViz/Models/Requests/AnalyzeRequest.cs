using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class AnalyzeRequest
    {
        public string PackageID { get; set; }
        public string Version { get; set; }
        public string Source { get; set; }
        public string Framework { get; set; }
        public string FrameworkVersion { get; set; }
        public string FrameworkProfile { get; set; } 
    }
}