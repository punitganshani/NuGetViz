using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuGetViz.Jobs
{
    public class JobScheduler
    {
        public static void Start()
        {
            var config = SharedInfo.Instance.GetConfig();

            if (config.Jobs.Count == 0)
                return;

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            foreach (var jobInfo in config.Jobs)
            {
                Type jobType = Type.GetType(jobInfo.JobType);
                if (jobType != null)
                {
                    IJobDetail job = JobBuilder.Create(jobType).Build();
                    scheduler.ScheduleJob(job, JobTriggers.CronJob(jobInfo.CRON));
                }
            }
        }
    }
}