using System.Web;
using System.Web.Configuration;
using Amdaris;
using CEC.Web.Results.Api.Dtos;
using CEC.Web.Results.Api.Infrastructure;
using FluentNHibernate.Utils;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog.Internal;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace CEC.Web.Results.Api.Controllers
{
    [HandleAndLogError]
    public class LiveResultsHandler : WebSocketHandler
    {
        private static WebSocketCollection _chatClients = new WebSocketCollection();
		private static StatsTimer ServerTimer = new StatsTimer();
         
        private ILogger _logger;
          
        public LiveResultsHandler(ILogger logger)
        { 
            _logger = logger;
        }

        public override void OnOpen()
        {
            //_chatClients.Add(this);

            //if (!ServerTimer.Enabled)
            //{
            //    InitDataOnIntervalTime();
            //    ServerTimer.Elapsed += FullNotification;
            //    ServerTimer.Enabled = true;
            //}
            
            //LoadVotingInfoSingle();
            //LogStats();
        }

        public override void OnClose()
        {
            _chatClients.Remove(this);
        }

        public override void OnMessage(string message)
        {

        }

        private void FullNotification(object sender, System.Timers.ElapsedEventArgs e)
        {
            int minutes = DateTime.Now.Minute;

            //if (minutes % ServerTimer.TimerIntervalConfig == 0)
            {
                InitDataOnIntervalTime();
                LoadVotingInfoToAll();
            }
        }

        private void LoadVotingInfoSingle()
        {
            ServerTimer.Info.TotalConnections = _chatClients.Count;

            string json = JsonConvert.SerializeObject(ServerTimer.Info);

            var singleItem = new WebSocketCollection {this};
            
            singleItem.Broadcast(json);
        }

        private void LoadVotingInfoToAll()
        {
            string json = JsonConvert.SerializeObject(ServerTimer.Info);

            _chatClients.Broadcast(json);
        }

        DateTime VotingHours(DateTime votingDay, int hourIn, int minuteIn)
        {
            return new DateTime(votingDay.Year, votingDay.Month, votingDay.Day, hourIn, minuteIn, 0);
        }

        private void InitDataOnIntervalTime()
        {
            var testingMode = false;
            bool.TryParse(WebConfigurationManager.AppSettings["TestingMode"], out testingMode);
            
            int votingHourEnd = ServerTimer.VoteEndHourConfig;
            StatsInfo info = ServerTimer.Info;

            if (info == null) info = new StatsInfo();

            info.Message = "Server Information";
            info.CurrentTime = DateTime.Now.ToShortTimeString();
            info.TotalConnections = _chatClients.Count;

            LiveResultsLogic LRL = new LiveResultsLogic { ElectionId = StatsTimer.ElectionsId };

            DateTime votingDate = LRL.VotingDayById();
            DateTime votingEnd = VotingHours(votingDate, votingHourEnd, 0);
            DateTime now = VotingHours(votingDate, DateTime.Now.Hour, DateTime.Now.Minute);

            // Get data from DB in case when Voting is not ended or if voting results are empty
            if (now <= votingEnd || info.BaseInfo == null || !info.BaseInfo.Any())
            {
                info.BaseInfo = GetBasePollingInfo(LRL);
                info.PercentInfo = GetPercentInfo(LRL, info.BaseInfo);

                if (info.BaseInfo.Any())
                {
                    info.TotalParticipants = info.BaseInfo.Last().VotersParticipateCount;
                }
            }

            // Begin collecting preliminaries after closing of all polling stations
            if (now > votingEnd || testingMode)
            {
                info.ResultsProcessingStarted = true;
                var votesData = LRL.GetTotalValidVotes();
                info.TotalValidVotes = votesData.TotalValidVotes;
                info.TotalSpoiledVotes = votesData.TotalSpoiledVotes;
                info.TotalVotes = votesData.TotalVotes;

                info.Preliminary = GetPreliminaries(LRL, votesData.TotalValidVotes);
                info.PreliminaryTotalCalculated = info.Preliminary.Sum(a => a.CandidateResult);
				

                BallotPapersCountData ballotInfo = LRL.GetBallotPapers();
                info.TotalBallotPapers = ballotInfo.TotalBallotPapers;
                info.TotalProcessedBallotPapers = ballotInfo.TotalProcessedBallotPapers;
            }
        } 

        private StatPercentInfo GetPercentInfo(LiveResultsLogic LRL, IEnumerable<StatBaseInfo> bsInfo)
        {
            long total = LRL.GeneralTotalVoters();

            var lastBi = bsInfo
                     .Where(bi => bi.VotersParticipateCount > 0)
                     .OrderByDescending(bi => bi.VotersParticipateCount)
                     .ToList().FirstOrDefault();

            long notVoted = 0;
            string lastTime = "";
            long votersParticipateCount = 0;

            if (lastBi != null)
            {
                notVoted = total - lastBi.VotersParticipateCount;
                lastTime = lastBi.TimeOfData;
                votersParticipateCount = lastBi.VotersParticipateCount;
            }

            return new StatPercentInfo
            {
                LastTime = lastTime,
                TotalCount = total,
                VotedCount = votersParticipateCount,
                VotedPercent = string.Format("{0:P}", total != 0 ? (double)votersParticipateCount/total : 0.0),
                NotVotedPercent = string.Format("{0:P}", total != 0 ? (double)notVoted/total : 0.0)
            };
        }

        private IEnumerable<StatBaseInfo> GetBasePollingInfo(LiveResultsLogic LRL)
        {
            List<StatBaseInfo> infos = new List<StatBaseInfo>();

            int hourBegin = ServerTimer.VoteStartHourConfig;
            int hourEnd = ServerTimer.VoteEndHourConfig;
            int interval = ServerTimer.TimerIntervalConfig; // minutes

            DateTime votingDate = LRL.VotingDayById();
            DateTime dtBegin = VotingHours(votingDate, hourBegin, 0);
            DateTime dtEnd = VotingHours(votingDate, hourEnd, 0);
            DateTime now = VotingHours(votingDate, DateTime.Now.Hour, DateTime.Now.Minute);
             
            List<VotersActivity> baseList = LRL.GeneralOnTimeResults(dtBegin, dtEnd, interval);
            List<VotersActivity> suplList = LRL.GeneralOnTimeAddResults(dtBegin, dtEnd, interval);

            long baseSum = baseList.Sum(x=>x.VotersCount);
            long suplSum = suplList.Sum(x=>x.VotersCount);

            for (int i = 0; i < baseList.Count; i++)
            {
                //baseSum += baseList[i].VotersCount;
                //suplSum += suplList[i].VotersCount;

                DateTime calcTime = VotingHours(votingDate, baseList[i].Hour, baseList[i].Minutes);

                if (now >= calcTime)
                {
                    infos.Add(new StatBaseInfo
                    {
                        TimeOfData = calcTime.ToShortTimeString(),
                        VotersByBaseList = (baseSum - suplSum),
                        VotersByAddList = (suplSum),
                        VotersParticipateCount = baseSum
                    });
                }
            }
            
            return infos;
        }

        //private IEnumerable<StatPreliminaryResult> GetPreliminaries(LiveResultsLogic lrl, long totalVotes, long independentCandidateMinimalThreshold)
        private IEnumerable<StatPreliminaryResult> GetPreliminaries(LiveResultsLogic lrl, long totalValidVotes)
        {
            var usePartyColors = true;
            bool.TryParse(ConfigurationManager.AppSettings["UsePartyColors"], out usePartyColors);
            var defaultColor = ConfigurationManager.AppSettings["PartyDefaulColor"];

            List<StatPreliminaryResult> list = lrl.GeneralPartidResults();

            if (!usePartyColors)
            {
                foreach (var statPreliminaryResult in list)
                {
                    statPreliminaryResult.CandidateColor = defaultColor;
                }
            }
            list = list.OrderBy(x => x.BallotOrder).ToList();
            //var passedLimit = 0;
            //list = list.OrderByDescending(a => a.CandidateResult).ToList();
            
            //for (var j = 0; j < list.Count; j++)
            //{
            //    if (list[j].CandidateResult > independentCandidateMinimalThreshold)
            //    {
            //        passedLimit ++;
            //    }
            //}
            //int maxCandidates = 14;
            //if (passedLimit > maxCandidates)
            //{
            //    passedLimit++;
            //    maxCandidates = passedLimit;
            //}
            ////const int maxCandidates = 10;
                
            //if (list.Count > maxCandidates)
            //{
            //    double sum = 0;
            //    for (int i = maxCandidates - 1; i < list.Count; i++)
            //    {
            //        sum += list[i].CandidateResult;
            //    }

            //    list.RemoveRange(maxCandidates, list.Count - maxCandidates);

            //    list[maxCandidates - 1] = new StatPreliminaryResult
            //    {
            //        CandidateName = "ALTELE",
            //        CandidateColor = defaultColor,
            //        CandidateResult = (int) sum
            //    };
            //}
           
            //list.ForEach(spr => spr.CandidatePercentResult = Math.Round(100.0 * spr.CandidateResult / totalVotes, 2));
            list.ForEach(
                spr => spr.CandidatePercentResult =
                        totalValidVotes == 0 
                        ? 0 
                        : (double)spr.CandidateResult/totalValidVotes);

            return list;
        }

        private void LogStats()
        {
            
            string clientIP = "<Unknown_IP>";
            string browserID = "<Unknown_Browser>";

            clientIP = WebSocketContext.UserHostAddress;
            browserID = WebSocketContext.UserAgent;

            _logger.Trace(string.Format("[IP:{0}, BROWSER:{1}, CONNECTED: {2}, APPMODULE: results]",
                clientIP, browserID, _chatClients.Count));
        }
    }
}