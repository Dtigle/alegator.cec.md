using System.Web;
using Amdaris;
using CEC.Web.Results.Api.Dtos;
using CEC.Web.Results.Api.Infrastructure;
using FluentNHibernate.Utils;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CEC.Web.Results.Api.Controllers
{
    [HandleAndLogError]
    public class LiveElectionTurnoutHandler : WebSocketHandler
    {
        private static WebSocketCollection _chatClients = new WebSocketCollection();
        private static StatsTimer ServerTimer = new StatsTimer();
         
        private ILogger _logger;

        public LiveElectionTurnoutHandler(ILogger logger)
        { 
            _logger = logger;
        }

        public override void OnOpen()
        {
            _chatClients.Add(this);

            if (!ServerTimer.Enabled)
            {
                InitDataOnIntervalTime();
                ServerTimer.Elapsed += FullNotification;
                ServerTimer.Enabled = true;
            }
            InitDataOnIntervalTime();
            LoadVotingInfoSingle();
            LogStats();
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
            if (e.SignalTime.Minute % ServerTimer.TimerIntervalConfig == 0)
            {
                InitDataOnIntervalTime();
                LoadVotingInfoToAll();
            }
        }

        private void LoadVotingInfoSingle()
        {
            string json = JsonConvert.SerializeObject(ServerTimer.TurnoutInfo);

            var singleItem = new WebSocketCollection {this};
            
            singleItem.Broadcast(json);
        }

        private void LoadVotingInfoToAll()
        {
            string json = JsonConvert.SerializeObject(ServerTimer.TurnoutInfo);

            _chatClients.Broadcast(json);
        }
        
        private void InitDataOnIntervalTime()
        {
            var turnoutInfo = new TurnoutInfo();
            int totalVotersInSupplimentarylist = 0;
            long totalVotersOnBaselist = 0;

            var LRL = new LiveResultsLogic { ElectionId = StatsTimer.ElectionsId };

            var pollingStationsInfo = LRL.GetAssignedPollingStations();

            if (pollingStationsInfo != null && pollingStationsInfo.Any())
            {
                turnoutInfo.TotalPollingStations = pollingStationsInfo.Count;
                foreach (var psInfo in pollingStationsInfo)
                {
                    var pollingStationTurnout = new PollingStationTurnout();
                    pollingStationTurnout.Number = psInfo.PollingStationNumber;
                    pollingStationTurnout.Circumscription = psInfo.Circumscription;
                    pollingStationTurnout.Locality = psInfo.Locality;
                    pollingStationTurnout.VotersOnBaseList = psInfo.OpeningVoters;
                    pollingStationTurnout.LocationLatitude = psInfo.LocationLatitude;
                    pollingStationTurnout.LocationLongitude = psInfo.LocationLongitude;
                    pollingStationTurnout.VotersReceivedBallots = psInfo.VotersReceivedBallots;
                    pollingStationTurnout.VotersOnSupplementaryList = psInfo.VotersInSupplimentaryList;
                    var votersInSupplimentarylist = psInfo.VotersInSupplimentaryList;
                    var voters = (pollingStationTurnout.VotersOnBaseList +
                                  votersInSupplimentarylist);
                    pollingStationTurnout.PercentCalculated = voters > 0 
                        ? string.Format("{0:P}", (double)pollingStationTurnout.VotersReceivedBallots / voters)
                        : string.Format("{0:P}", 0);

                    totalVotersOnBaselist += pollingStationTurnout.VotersOnBaseList;
                    totalVotersInSupplimentarylist += votersInSupplimentarylist;
                    turnoutInfo.TotalVotersReceivedBallots += pollingStationTurnout.VotersReceivedBallots;

                    turnoutInfo.PollingStationsTurnout.Add(pollingStationTurnout);
                }
                turnoutInfo.TotalVotersOnBaseList = totalVotersOnBaselist;
                turnoutInfo.TotalVotersOnSupplementaryList = totalVotersInSupplimentarylist;
                var totVoters = turnoutInfo.TotalVotersOnBaseList +
                                totalVotersInSupplimentarylist;
                turnoutInfo.TotalPercentCalculated = totVoters > 0
                        ? string.Format("{0:P}", (double)turnoutInfo.TotalVotersReceivedBallots / totVoters)
                        : string.Format("{0:P}", 0);

                ServerTimer.TurnoutInfo = turnoutInfo;
            }
        }

        private void LogStats()
        {

            string clientIP = "<Unknown_IP>";
            string browserID = "<Unknown_Browser>";

            clientIP = WebSocketContext.UserHostAddress;
            browserID = WebSocketContext.UserAgent;

            _logger.Trace(string.Format("[IP:{0}, BROWSER:{1}, CONNECTED: {2}, APPMODULE: turnout]",
                clientIP, browserID, _chatClients.Count));
        }
    }
}