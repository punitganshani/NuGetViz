using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class ViewRequest
    {
        public string PackageID { get; set; }
        public string Source { get; set; }
    }
}