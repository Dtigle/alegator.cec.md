using System.Linq;
using AutoMapper;
using CEC.SRV.Domain;
using CEC.Web.SRV.Models.Account;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Profiles
{
    public class IdentityUserProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<SRVIdentityUser, UserGridModel>()
             .ForMember(x => x.Login, opt => opt.MapFrom(src => src.UserName))
             .ForMember(x => x.FullName,
                 opt =>
                     opt.MapFrom(src => src.AdditionalInfo != null ? src.AdditionalInfo.FullName : string.Empty))
             .ForMember(x => x.Role,
                 opt =>
                     opt.MapFrom(src => src.Roles.FirstOrDefault() != null ? src.Roles.First().Name : string.Empty))
             .ForMember(x => x.LastLogin,
                 opt =>
                     opt.MapFrom(
                         src => src.LastLogin.HasValue ? src.LastLogin.Value.LocalDateTime.ToString() : string.Empty))
             .ForMember(x => x.Status, opt => opt.MapFrom(src => src.IsBlocked ? MUI.UserBlocked : MUI.UserActive))
             .ForMember(x => x.Regions,
                 opt => opt.MapFrom(src => string.Join(";", src.Regions.Select(y => y.GetFullName()))));
        }
    }
}