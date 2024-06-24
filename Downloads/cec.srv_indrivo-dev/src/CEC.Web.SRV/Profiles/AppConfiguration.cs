using System.Globalization;
using AutoMapper;
using CEC.SRV.Domain;
using CEC.Web.SRV.Models.AppConfiguration;

namespace CEC.Web.SRV.Profiles
{
    public class AppConfigurationProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ConfigurationSetting, ConfigurationSettingsGridModel>()
                .ForMember(x => x.DataCreated,
                    opt =>
                        opt.MapFrom(
                            src =>
                                (src.Created.HasValue
                                    ? src.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                                    : string.Empty)))
                .ForMember(x => x.DataModified,
                    opt =>
                        opt.MapFrom(
                            src =>
                                (src.Modified.HasValue
                                    ? src.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                                    : string.Empty)))
                .ForMember(x => x.DataDeleted,
                    opt =>
                        opt.MapFrom(
                            src =>
                                (src.Deleted.HasValue
                                    ? src.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                                    : string.Empty)))

                .ForMember(x => x.CreatedById,
                    opt => opt.MapFrom(src => (src.CreatedBy != null ? src.CreatedBy.UserName : string.Empty)))
                .ForMember(x => x.ModifiedById,
                    opt => opt.MapFrom(src => (src.ModifiedBy != null ? src.ModifiedBy.UserName : string.Empty)))
                .ForMember(x => x.DeletedById,
                    opt => opt.MapFrom(src => (src.DeletedBy != null ? src.DeletedBy.UserName : string.Empty)))
                ;
        }
    }
}