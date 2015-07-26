using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NuGet.Frameworks;
using System.ComponentModel.DataAnnotations;

namespace NuGetViz.Models
{
    public class NuGetFrameworkInfo
    {
        public string Framework { get; set; }
        public bool IsAnyVersion { get; set; }

        public string Profile { get; set; }
        public bool IsPortable { get; set; }

        public string Version { get; set; }
        public string HtmlVersion { get; private set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? DependencyCount { get; internal set; }

        public static NuGetFrameworkInfo Any()
        {
            return new NuGetFrameworkInfo
            {
                Framework = "Any",
                IsAnyVersion = true,
                Version = "v0.0.0.0",
                HtmlVersion = string.Empty
            };
        }

        public static NuGetFrameworkInfo Any(string version)
        {
            return new NuGetFrameworkInfo
            {
                Framework = "Any",
                IsAnyVersion = true,
                Version = version,
                HtmlVersion = string.Empty
            };
        }

        public string ToHtmlString()
        {
            if (string.IsNullOrEmpty(HtmlVersion))
            {
                return Framework;
            }
            else
            {
                return String.Format("{0}, {1}", Framework, HtmlVersion);
            }
        }

        internal static NuGetFrameworkInfo CreateFrom(NuGetFramework targetFramework)
        {
            if (targetFramework.IsAny || targetFramework.IsAgnostic)
                return NuGetFrameworkInfo.Any(targetFramework.Version.ToString());

            var fxInfo = new NuGetFrameworkInfo();
            fxInfo.Framework = targetFramework.Framework;
            fxInfo.IsAnyVersion = false;
            fxInfo.Version = targetFramework.Version.ToString();
            fxInfo.HtmlVersion = CreateHtmlFriendlyVersion(targetFramework.Version);
            fxInfo.IsPortable = targetFramework.IsPCL;
            fxInfo.Profile = targetFramework.Profile;
            return fxInfo;
        }

        private static string CreateHtmlFriendlyVersion(Version version)
        {
            if (version.Major == 0 && version.Minor == 0 && version.Build == 0)
                return string.Empty;

            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}