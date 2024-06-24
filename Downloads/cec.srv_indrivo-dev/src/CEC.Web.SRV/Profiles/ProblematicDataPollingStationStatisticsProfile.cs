using AutoMapper;
using CEC.SRV.Domain;
using CEC.Web.SRV.Models.Statistics;

namespace CEC.Web.SRV.Profiles
{
	public class ProblematicDataPollingStationStatisticsProfile : Profile
    {
        protected override void Configure()
        {
			Mapper.CreateMap<ProblematicDataPollingStationStatistics, ProblematicDataPollingStationGridModel>()
                .ForMember(x => x.RegionName, y => y.MapFrom(x => x.FullRegionName))
                .ForMember(x => x.PollingStation, y => y.MapFrom(x => x.PollingStation))
				.ForMember(x => x.VotersCount, y => y.MapFrom(x => x.VotersCount));
        }
    }
}