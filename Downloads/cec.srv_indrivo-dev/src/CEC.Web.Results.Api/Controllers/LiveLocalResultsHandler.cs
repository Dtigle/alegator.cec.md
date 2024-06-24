using System.Linq;
using System.Timers;
using Amdaris;
using CEC.Web.Results.Api.Dtos;
using CEC.Web.Results.Api.Infrastructure;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CEC.Web.Results.Api.Controllers
{
	[HandleAndLogError]
    public class LiveLocalResultsHandler : WebSocketHandler
    {
        private static readonly WebSocketCollection ChatClients = new WebSocketCollection();
		private static readonly Timer ServerTimer = new Timer() { Enabled = false, Interval = 10 * 1000 };
         
        private readonly ILogger _logger;
	    private readonly List<LiveResultsLogic> _resultsReaders;

		private static IEnumerable<LocalElectionResult> Info { get; set; }

		public LiveLocalResultsHandler(ILogger logger)
		{
			_logger = logger;

            _resultsReaders = new List<LiveResultsLogic>
            {
                new LiveResultsLogic { ElectionId = 59 },
                new LiveResultsLogic { ElectionId = 60 },
                new LiveResultsLogic { ElectionId = 62 },
                new LiveResultsLogic { ElectionId = 63 },
                new LiveResultsLogic { ElectionId = 64 }
            };
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
	            var result = _resultsReaders
	                .Select(x => x.GetBallotPapers())
	                .Select(x =>
	                    new LocalElectionResult
	                    {
	                        TotalPollingStations = x.TotalBallotPapers,
	                        BallotPapersCount = x.TotalProcessedBallotPapers
	                    }).ToList();
	            Info = result;

	        }
	        catch (Exception ex)
	        {
                _logger.Error(ex);
	        }
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