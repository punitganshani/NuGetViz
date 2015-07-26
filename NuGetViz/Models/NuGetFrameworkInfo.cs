using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NuGet.Frameworks;

namespace NuGetViz.Models
{
    public class NuGetFrameworkInfo
    {
        public string Framework { get; set; }
        public bool IsAnyVersion { get; set; }

        public string Profile { get; set; }
        public bool IsPortable { get; set; }

        public string Version { get; set; }

        public static NuGetFrameworkInfo Any()
        {
            return new NuGetFrameworkInfo
            {
                Framework = "Any",
                IsAnyVersion = true,
                Version = string.Empty
            };
        }

        public string ToHtmlString()
        {
            if (string.IsNullOrEmpty(Version))
            {
                return Framework;
            }
            else
            {
                return String.Format("{0}, {1}", Framework, Version);
            }
        }

        internal static NuGetFrameworkInfo CreateFrom(NuGetFramework targetFramework)
        {
            if (targetFramework.IsAny || targetFramework.IsAgnostic)
                return NuGetFrameworkInfo.Any();

            var fxInfo = new NuGetFrameworkInfo();
            fxInfo.Framework = targetFramework.Framework;
            fxInfo.IsAnyVersion = false;
            fxInfo.Version = targetFramework.Version.ToString();
            fxInfo.IsPortable = targetFramework.IsPCL;
            fxInfo.Profile = targetFramework.Profile;
            return fxInfo;
        }
    }
}