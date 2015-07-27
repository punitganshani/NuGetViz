using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NuGetViz.Jobs
{
    public class CleanDownloadFolder : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var config = SharedInfo.Instance.GetConfig();
            Directory.Delete(config.PackageDownloadFolderName, true);
        }
    }
}