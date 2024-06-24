using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using CEC.SRV.BLL.Quartz;
using CEC.Web.SRV.Models.QuartzAdmin;

namespace CEC.Web.SRV.Profiles
{
    public class QuartzDataProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<QuartzJobData, QuartzJobModel>()
                .ForMember(x => x.TriggerGroup, y => y.MapFrom(z => z.TriggerGroupName))
                .Include<QuartzJobData, QuartzScheduledJobsGridModel>()
                .Include<QuartzJobData, QuartzRunningJobsGridModel>();

            Mapper.CreateMap<QuartzJobData, QuartzScheduledJobsGridModel>();
            Mapper.CreateMap<QuartzJobData, QuartzRunningJobsGridModel>()
                .ForMember(x => x.RunTime, y => y.MapFrom(z => z.GetRunTime()));

            Mapper.CreateMap<JobProgress, JobProgressModel>();
            Mapper.CreateMap<ProgressInfo, ProgressInfoModel>();
        }
    }
}