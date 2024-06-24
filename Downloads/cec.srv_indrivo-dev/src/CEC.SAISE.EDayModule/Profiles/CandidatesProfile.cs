using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using CEC.SAISE.BLL.Dto.Concurents;
using CEC.SAISE.EDayModule.Models.PoliticalParty;

namespace CEC.SAISE.EDayModule.Profiles
{
    public class CandidatesProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<DelimitationModel, DelimitationDto>();
            Mapper.CreateMap<DeleteCandidateModel, DeleteCandidateDto>();
            Mapper.CreateMap<UpdateCandidateOrderModel, UpdateCandidateOrderDto>();
            Mapper.CreateMap<PoliticalPartyModel, PoliticalPartyDto>();
            Mapper.CreateMap<CandidateModel, CandidateDto>()
                .ForMember(x => x.Idnp, z => z.MapFrom(y => long.Parse(y.Idnp)));

            Mapper.CreateMap<PoliticalPartyDto, PoliticalPartyModel>();
            Mapper.CreateMap<CandidateDto, CandidateModel>()
                .ForMember(x => x.Idnp, z => z.MapFrom(y => y.Idnp.ToString("D13")));

            Mapper.CreateMap<AllocationItemModel, AllocationItemDto>();
        }
    }
}