using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Timers;
using Amdaris;
using CEC.Web.Results.Api.Dtos;
using CEC.Web.Results.Api.Infrastructure;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;

namespace CEC.Web.Results.Api.Controllers
{
	[HandleAndLogError]
	public class LiveMunicipalResultsHandler : WebSocketHandler
	{
		private static readonly WebSocketCollection ChatClients = new WebSocketCollection();
		private static readonly Timer ServerTimer = new Timer() { Enabled = false, Interval = 30 * 1000 };
         
		private readonly ILogger _logger;
		private readonly LiveResultsLogic _liveResults1;
		//private readonly LiveResultsLogic _liveResults2;

		private static IEnumerable<LocalStatsInfo> Info { get; set; }

		public LiveMunicipalResultsHandler(ILogger logger)
		{
			_logger = logger;

			_liveResults1 = new LiveResultsLogic { ElectionId = 60 };
			//_liveResults2 = new LiveResultsLogic { ElectionId = 58 };
		}

		public override void OnOpen()
		{
			ChatClients.Add(this);

			if (!ServerTimer.Enabled)
			{
				InitDataOnIntervalTime();
				ServerTimer.Elapsed += FullNotification;
				ServerTimer.Enabled = true;
			}
            
			LoadPreliminaryResultsInfoSingle();
			LogStats();
		}

		public override void OnClose()
		{
			ChatClients.Remove(this);
		}

		public override void OnMessage(string message)
		{

		}

		private void FullNotification(object sender, System.Timers.ElapsedEventArgs e)
		{
			InitDataOnIntervalTime();
			LoadPreliminaryResultsInfoToAll();
		}

		private void LoadPreliminaryResultsInfoSingle()
		{
			var json = JsonConvert.SerializeObject(Info);

			var singleItem = new WebSocketCollection {this};
            
			singleItem.Broadcast(json);
		}


		private void LoadPreliminaryResultsInfoToAll()
		{
			var json = JsonConvert.SerializeObject(Info);

			ChatClients.Broadcast(json);
		}

		private void InitDataOnIntervalTime()
		{
		    try
		    {
		        var list = new List<LocalStatsInfo>
		        {
		            GetStatsInfo(_liveResults1, 44),
                    //GetStatsInfo(_liveResults1, 45),
                    //GetStatsInfo(_liveResults2, 44),
                    //GetStatsInfo(_liveResults2, 45)
		        };

		        Info = list;
		    }
		    catch (Exception ex)
		    {
                _logger.Error(ex);
		    }
		}

		private LocalStatsInfo GetStatsInfo(LiveResultsLogic LRL, long districId)
		{
			var info = new StatsInfo();

			info.ResultsProcessingStarted = true;
			var votesData = LRL.GetTotalValidVotesForDistrict(districId);
			info.TotalValidVotes = votesData.TotalValidVotes;
			info.TotalSpoiledVotes = votesData.TotalSpoiledVotes;
			info.TotalVotes = votesData.TotalVotes;

			info.Preliminary = GetPreliminaries(LRL, votesData.TotalValidVotes, districId);
			info.PreliminaryTotalCalculated = info.Preliminary.Sum(a => a.CandidateResult);
			
			var ballotInfo = LRL.GetBallotPapersForDistrict(districId);
			info.TotalBallotPapers = ballotInfo.TotalBallotPapers;
			info.TotalProcessedBallotPapers = ballotInfo.TotalProcessedBallotPapers;
			
			return new LocalStatsInfo(){ DistrictId = districId, ElectionId= LRL.ElectionId, StatsInfo = info };
		}

		private IEnumerable<StatPreliminaryResult> GetPreliminaries(LiveResultsLogic lrl, long totalValidVotes, long districId)
		{
			var usePartyColors = true;
			bool.TryParse(ConfigurationManager.AppSettings["UsePartyColors"], out usePartyColors);
			var defaultColor = ConfigurationManager.AppSettings["PartyDefaulColor"];

			List<StatPreliminaryResult> list = lrl.GeneralPartidResultsForDistrict(districId);

			if (!usePartyColors)
			{
				foreach (var statPreliminaryResult in list)
				{
					statPreliminaryResult.CandidateColor = defaultColor;
				}
			}
			list = list.OrderBy(x => x.BallotOrder).ToList();

			list.ForEach(
				spr => spr.CandidatePercentResult =
						totalValidVotes == 0
						? 0
						: (double)spr.CandidateResult / totalValidVotes);

			return list;
		}

		private void LogStats()
		{
            
			string clientIP = "<Unknown_IP>";
			string browserID = "<Unknown_Browser>";

			clientIP = WebSocketContext.UserHostAddress;
			browserID = WebSocketContext.UserAgent;

			_logger.Trace(string.Format("[IP:{0}, BROWSER:{1}, CONNECTED: {2}, APPMODULE: results]",
				clientIP, browserID, ChatClients.Count));
		}
	}
}