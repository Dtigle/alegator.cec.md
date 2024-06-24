using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Models.QuartzAdmin
{
    public class JobProgressModel
    {
        public JobProgressModel()
        {
            StageInfos = new List<ProgressInfoModel>();
        }

        public ProgressInfoModel OverallProgress { get; set; }

        public List<ProgressInfoModel> StageInfos { get; set; }
    }
}