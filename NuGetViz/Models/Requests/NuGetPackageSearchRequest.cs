using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class NuGetPackageSearchRequest
    {
        public string SearchKeyword { get; set; }

        public string SourceUrl { get; set; }

        public string Source { get; set; }
    } 
    
      
}