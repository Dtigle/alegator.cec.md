using AutoMapper;
using CEC.SRV.Domain;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Election;
using System.Globalization;
using System.Linq;

namespace CEC.Web.SRV.Profiles
{
    public class ElectionProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Election, ElectionGridModel>()
                .ForMember(x => x.ElectionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(x => x.DataCreated,
                    opt =>
                        opt.MapFrom(
                            src => (src.Created.HasValue ? src.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty)))
                .ForMember(x => x.DataModified,
                    opt =>
                        opt.MapFrom(
                            src => (src.Modified.HasValue ? src.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty)))
                .ForMember(x => x.DataDeleted,
                    opt =>
                        opt.MapFrom(
                            src => (src.Deleted.HasValue ? src.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty)))

                .ForMember(x => x.CreatedById,
                    opt => opt.MapFrom(src => (src.CreatedBy != null ? src.CreatedBy.UserName : string.Empty)))
                .ForMember(x => x.ModifiedById,
                    opt => opt.MapFrom(src => (src.ModifiedBy != null ? src.ModifiedBy.UserName : string.Empty)))
                .ForMember(x => x.DeletedById,
                    opt => opt.MapFrom(src => (src.DeletedBy != null ? src.DeletedBy.UserName : string.Empty)))

                .ForMember(x => x.ElectionStatus, opt => opt.MapFrom(src => src.Status.GetEnumDescription()))
                .ForMember(x => x.ElectionType, z => z.MapFrom(y => y.ElectionType.Name))
                .ForMember(x => x.ElectionDate, z => z.MapFrom(y => y.ElectionRounds.FirstOrDefault().ElectionDate.ToShortDateString()))
                .ForMember(x => x.Description, z => z.MapFrom(y => y.Description));

            Mapper.CreateMap<Election, UpdateElectionModel>()
                .ForMember(x => x.Id, z => z.MapFrom(y => y.Id))
                .ForMember(x => x.ElectionType, z => z.MapFrom(y => y.ElectionType.Id))
                .ForMember(x => x.ElectionDate, z => z.MapFrom(y => y.StatusDate.DateTime))
                .ForMember(x => x.Comments, z => z.MapFrom(y => y.Description));
        }
    }
}