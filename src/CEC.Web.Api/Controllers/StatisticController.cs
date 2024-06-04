using CEC.SRV.BLL;
using CEC.Web.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace CEC.Web.Api.Controllers
{
    [RoutePrefix("Statistic")]
    [NHibernateSession]
    public class StatisticController : ApiController
    {
        private readonly IVotersBll _votersBll;

        public StatisticController(IVotersBll votersBll)
        {
            _votersBll = votersBll;
        }

        [Route("Statistic")]
        public IEnumerable<StatisticInfo> GetAssigned()
        {
            var voters = _votersBll.GetAbroadVotersAddress();
            return voters.Select(x => new StatisticInfo {Lat = x.AbroadAddressLat, Long = x.AbroadAddressLong});
        }

        [Route("StatisticGrouped")]
        public IEnumerable<StatisticGrouped> GetVotersRegistration()
        {
            var queryResult = _votersBll.GetRegionOfVotes();
            return queryResult.Select(x => new StatisticGrouped() { Country = x.Country, Count = x.Count });
        }

        [Route("TimelineStatistics")]
        public IEnumerable<StatisticDateGrouped> GetTimelineStatistics()
        {
            var queryResult = _votersBll.GetAbroadVotersTimeline();
            return queryResult.Select(x => new StatisticDateGrouped() { Date = x.Date, Count = x.Count });
        }
    }
}
