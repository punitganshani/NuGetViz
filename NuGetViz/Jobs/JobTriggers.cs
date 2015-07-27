using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Jobs
{
    public class JobTriggers
    {
        public static ITrigger CronJob(string cron)
        {
            return TriggerBuilder.Create()
                .WithCronSchedule(cron, s => s.WithMisfireHandlingInstructionDoNothing())
                .Build();
        }

    }
}