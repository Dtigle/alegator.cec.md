using AutoMapper;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain.Importer;
using CEC.Web.SRV.Models.Statistics;

namespace CEC.Web.SRV.Profiles
{
    public class StatisticsProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ImportStatisticsGridDto, ImportsGridModel>()
                .ForMember(x => x.DataExecutionImport, y => y.MapFrom(x => x.ImportDateTime))
                .ForMember(x => x.Id, y => y.MapFrom(x => x.ImportDateTime))
                ;
            Mapper.CreateMap<ImportStatisticsDto, ImportStatisticsModel>()
                .ForMember(x => x.New, y => y.MapFrom(x => x.New))
                .ForMember(x => x.Conflicted, y => y.MapFrom(x => x.Conflicted))
                .ForMember(x => x.Updated, y => y.MapFrom(x => x.Updated))
                .ForMember(x => x.Error, y => y.MapFrom(x => x.Error))
                .ForMember(x => x.Total, y => y.MapFrom(x => x.Total))
                .ForMember(x => x.ChangedStatus, y => y.MapFrom(x => x.ChangedStatus))
                .ForMember(x => x.ResidenceChnaged, y => y.MapFrom(x => x.ResidenceChanged))
                ;
        }
    }
}