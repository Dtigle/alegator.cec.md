using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using CEC.SRV.Domain;
using CEC.Web.Api.Dtos.People;

namespace CEC.Web.Api.Profiles
{
    public class PeopleProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Person, PersonData>()
                .ForMember(x => x.Idnp, z => z.MapFrom(y => y.Idnp))
                .ForMember(x => x.FirstName, z => z.MapFrom(y => y.FirstName))
                .ForMember(x => x.LastName, z => z.MapFrom(y => y.Surname))
                .ForMember(x => x.Patronymic, z => z.MapFrom(y => y.MiddleName))
                .ForMember(x => x.DateOfBirth, z => z.MapFrom(y => y.DateOfBirth))
                .ForMember(x => x.Gender, z => z.MapFrom(y => y.Gender.Id));
        }
    }
}