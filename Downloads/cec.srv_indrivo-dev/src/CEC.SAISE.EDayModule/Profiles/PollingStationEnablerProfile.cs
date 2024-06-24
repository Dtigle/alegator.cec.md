using AutoMapper;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.EDayModule.Models.PermissionManage;
using CEC.SAISE.EDayModule.Models.VotingProcessStats;
using System;
using System.Globalization;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.EDayModule.Models.DocumentsGrid;

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
                    y => y.ResolveUsing(x => x.BallotPaperId ?? null))
            .ForMember(x => x.ElectionStartTime, y => y.MapFrom(x => x.ElectionStartTime.HasValue
                ? DateTime.Today.Add(x.ElectionStartTime.Value).ToString("HH:mm")
                : ""))
            .ForMember(x => x.ElectionEndTime, y => y.MapFrom(x => x.ElectionEndTime.HasValue
                ? DateTime.Today.Add(x.ElectionEndTime.Value).ToString("HH:mm")
                : ""));

            Mapper.CreateMap<OptionsToggleModel, OptionsToggleDto>();

            Mapper.CreateMap<VotingProcessStatsDto, VotingProcessStatsGridModel>()
                .ForMember(x => x.BallotPaperStatus,
                    y => y.ResolveUsing(x => x.BallotPaperStatus.HasValue
                        ? PoliticalPartyStatusExtension.GetEnumDescription(x.BallotPaperStatus.Value)
                        : "PV lipsește"));

            Mapper.CreateMap<UpdatePollingStationActivityDto, UpdatePollingStationActivityModel>()
                .ForMember(dest => dest.ElectionStartTime, opt => opt.MapFrom(src => src.ElectionStartTime.ToString("HH:mm")))
                .ForMember(dest => dest.ElectionEndTime, opt => opt.MapFrom(src => src.ElectionEndTime.ToString("HH:mm")));


            Mapper.CreateMap<UpdatePollingStationActivityModel, UpdatePollingStationActivityDto>()
                .ForMember(x => x.ElectionStartTime, y => y.MapFrom(src => ParseTimeOrFallback(src.ElectionStartTime)))
                .ForMember(x => x.ElectionEndTime, y => y.MapFrom(src => ParseTimeOrFallback(src.ElectionEndTime)));

            Mapper.CreateMap<UpdatePollingStationActivityDto, UpdatePollingStationActivityModel>();
            Mapper.CreateMap<UpdatePollingStationActivityModel, UpdatePollingStationActivityDto>();

            Mapper.CreateMap<PollintStationDocumentStageDto, PollingStationDocumentStageGridModel>();
            Mapper.CreateMap<PollingStationDocumentStageGridModel, PollintStationDocumentStageDto>();
        }
        // Custom parsing function
        private TimeSpan ParseTimeOrFallback(string timeString)
        {
            if (TimeSpan.TryParseExact(timeString, "h\\:mm", CultureInfo.InvariantCulture, out TimeSpan parsedTime))
            {
                return parsedTime;
            }
            return TimeSpan.Zero; // Fallback value

        }
    }
}