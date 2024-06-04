using System.Linq;
using CEC.Web.Results.Api.Dtos;
using System;
using System.Collections.Generic;

namespace CEC.Web.Results.Api.Infrastructure
{

    public class LiveResultsLogic
    {
        SqlFactory Factory;

        public int ElectionId { get; set; }

        public LiveResultsLogic()
        {
            Factory = new SqlFactory();
        }

        public List<VotersActivity> GeneralOnTimeResults(DateTime openVote, DateTime currentTime, int interval)
        {
            return Factory.GeneralOnTimeResults(ElectionId, openVote, currentTime, interval);
        }

        public List<VotersActivity> GeneralOnTimeAddResults(DateTime openVote, DateTime currentTime, int interval)
        {
            return Factory.GeneralOnTimeAddResults(ElectionId, openVote, currentTime, interval);
        }

        public int GeneralTotalVoters()
        {
            return Factory.GeneralTotalVoters(ElectionId);
        }

        public List<StatPreliminaryResult> GeneralPartidResults()
        {
			//var listGpr = Factory.GeneralPartidResults(ElectionId);
			//var listSpr = new List<StatPreliminaryResult>();
            
			//listGpr.ForEach(listSpr.Add); 

			//return listSpr;
	        return new List<StatPreliminaryResult>(Factory.GeneralPartidResults(ElectionId));
        }

		public List<StatPreliminaryResult> GeneralPartidResultsForDistrict(long districtId)
		{
			//var listGpr = Factory.GeneralPartidResultsForDistrict(ElectionId, districtId);
			//var listSpr = new List<StatPreliminaryResult>();

			//listGpr.ForEach(listSpr.Add);

			//return listSpr;

			return new List<StatPreliminaryResult>(Factory.GeneralPartidResultsForDistrict(ElectionId, districtId));
		}

        public DateTime VotingDayById()
        {
            return Factory.VotingDayById(ElectionId);
        }

        public byte[] LogoByCandidateId(int id)
        {
            return Factory.LogoByCandidateId(id);
        }

        public static string HexToWebImage(byte[] hex)
        {
            if (hex == null || hex.Length == 0) return string.Empty;

            string base64 = Convert.ToBase64String(hex);

            return string.Format("data:image/jpeg;base64,{0}", base64);
        }

        public List<AssignedPollingStation> GetAssignedPollingStations()
        {
            return Factory.GetAssignedPollingStations(ElectionId);
        } 
        public int GetTotalVotersReceivedBallotsPerPollingStation(long pollingStation)
        {
            return Factory.GetTotalVotersReceivedBallotsPerPollingStation(ElectionId, pollingStation);
        }  
        public int GetTotalVotersInSupplimentaryListPerPollingStation(long pollingStation)
        {
            return Factory.GetTotalVotersInSupplimentaryListPerPollingStation(ElectionId, pollingStation);
        }

        public BallotPapersCountData GetBallotPapers()
        {
            return Factory.GetBallotPapers(ElectionId).FirstOrDefault();
        }

		public BallotPapersCountData GetBallotPapersForDistrict(long districtId)
		{
			return Factory.GetBallotPapersForDistrict(ElectionId, districtId).FirstOrDefault();
		}

        public TotalVotesData GetTotalValidVotes()
        {
            return Factory.GetTotalValidVotes(ElectionId).FirstOrDefault();
        }

	    public TotalVotesData GetTotalValidVotesForDistrict(long districtId)
	    {
			return Factory.GetTotalValidVotesForDistrict(ElectionId, districtId).FirstOrDefault();
	    }
    }
}