using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.EDayModule.Models.ElectionResults;

namespace CEC.SAISE.EDayModule.Profiles
{
    public class ElectionResultsProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<CompetitorResultDto, CompetitorResult>()
                .ForMember(x => x.PoliticalPartyName,
                    z => z.MapFrom(y => HttpUtility.JavaScriptStringEncode(y.PoliticalPartyName)));

            Mapper.CreateMap<BallotPaperDataDto, BallotPaperDataModel>()
                .Include<BallotPaperDto, BallotPaperModel>();

            Mapper.CreateMap<BallotPaperDto, BallotPaperModel>();

            Mapper.CreateMap<BallotPaperDataModel, BallotPaperDataDto>();
            Mapper.CreateMap<CompetitorResult, CompetitorResultDto>();
        }
    }
}