using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using AutoMapper;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.EDayModule.Models;
using CEC.SAISE.EDayModule.Models.Voting;

namespace CEC.SAISE.EDayModule.Profiles
{
    public class UserDataProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<UserDataDto, UserDataModel>();

            Mapper.CreateMap<ValueNamePair, ValueNameModel>()
                .ForMember(x => x.Id, z => z.MapFrom(y => y.Id))
                .ForMember(x => x.Name, z => z.MapFrom(y => HttpUtility.JavaScriptStringEncode(y.Name)));
        }
    }
}