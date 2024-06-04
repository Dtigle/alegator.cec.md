using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.EDayModule.Models.PermissionManage;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Models.VotingProcessStats;

namespace CEC.SAISE.EDayModule.Profiles
{
	public class PollingStationEnablerProfile : Profile
	{
		protected override void Configure()
		{
		    Mapper.CreateMap<PollingStationStageEnablerDto, PollingStationStageEnablerGridModel>()
		        .ForMember(x => x.BallotPaperStatus,
		            y => y.ResolveUsing(x => x.BallotPaperStatus.HasValue
		                ? PoliticalPartyStatusExtension.GetEnumDescription(x.BallotPaperStatus.Value)
		                : "PV lipsește"))
            .ForMember(x => x.BallotPaperId,
                    y => y.ResolveUsing(x => x.BallotPaperId ?? null));

            Mapper.CreateMap<OptionsToggleModel, OptionsToggleDto>();

            Mapper.CreateMap<VotingProcessStatsDto, VotingProcessStatsGridModel>()
                .ForMember(x => x.BallotPaperStatus,
                    y => y.ResolveUsing(x => x.BallotPaperStatus.HasValue
                        ? PoliticalPartyStatusExtension.GetEnumDescription(x.BallotPaperStatus.Value)
                        : "PV lipsește"));
		}
	}
}