using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Models
{
    public class SearchRequest
    {
        public string SearchTerm { get; set; }
        public string Source { get; set; }
    }
}