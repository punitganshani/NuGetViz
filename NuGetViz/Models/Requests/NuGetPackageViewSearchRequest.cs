using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class NuGetPackageViewSearchRequest
    {
        public string PackageID { get; set; }

        public string SourceUrl { get; set; }

        public string Source { get; set; }
    }    
      
}