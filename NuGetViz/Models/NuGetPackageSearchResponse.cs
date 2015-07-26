using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class NuGetPackageSearchResponse
    {
        public NuGetPackageSearchRequest Request { get; set; }

        public List<NuGetPackageSearchResult> Results { get; set; }

        public NuGetPackageSearchResponse()
        {
            Request = new NuGetPackageSearchRequest();
            Results = new List<NuGetPackageSearchResult>();
        }
    }
}