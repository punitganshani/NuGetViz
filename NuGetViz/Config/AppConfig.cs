using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Core
{
    public class AppConfig
    {
        public List<NuGetRepo> Repositories { get; set; }
        public AppConfig()
        {
            Repositories = new List<NuGetRepo>();
        }

    }
}