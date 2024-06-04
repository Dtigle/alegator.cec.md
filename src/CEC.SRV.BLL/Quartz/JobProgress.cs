using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace CEC.SRV.BLL.Quartz
{
    [Serializable]
    public class JobProgress
    {
        public const string JobContextKey = "JobProgress";
        private readonly List<ProgressInfo> _stageInfos;
        private ProgressInfo _overallProgress;

        public JobProgress()
        {
            _stageInfos = new List<ProgressInfo>();
            _overallProgress = new ProgressInfo();
        }

        public ProgressInfo OverallProgress
        {
            get { return _stageInfos.Count == 1 ? _stageInfos.First() : _overallProgress; }
        }

        public IEnumerable<ProgressInfo> StageInfos 
        {
            get { return _stageInfos; }
        }

        /// <summary>
        /// Creates a new ProgressInfo instance for a job Stage and registers it to OverallProgress
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ProgressInfo CreatStageProgressInfo(string comment, int minimum, int maximum, int value = 0)
        {
            var stageInfo = new ProgressInfo(comment, minimum, maximum, value, OverallProgressIncrease);
            _stageInfos.Add(stageInfo);
            _overallProgress = new ProgressInfo(_overallProgress.Comments, 
                _overallProgress.Minimum,
                _overallProgress.Maximum + 1,
                _overallProgress.Value);

            return stageInfo;
        }

        private void OverallProgressIncrease()
        {
            OverallProgress.Increase();
        }
    }
}
