using System.Globalization;
using AutoMapper;
using CEC.SRV.Domain.Interop;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Interop;

namespace CEC.Web.SRV.Profiles
{
    public class InteropProfile : Profile
    {
        protected override void Configure()
        {

            Mapper.CreateMap<InteropSystem, InteropSystemGridModel>()
                .ForMember(x => x.TransactionProcessingType, z => z.MapFrom(y => EnumHelper.GetEnumDescription(y.TransactionProcessingType)))
                .ForMember(x => x.PersonStatusType, z => z.MapFrom(y => y.PersonStatusType != null ? y.PersonStatusType.Name : string.Empty))
                .ForMember(x => x.DataDeleted,
                    opt =>
                        opt.MapFrom(
                            src =>
                                (src.Deleted.HasValue
                                    ? src.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                                    : string.Empty)))
                ;

            Mapper.CreateMap<InteropSystem, UpdateInteropSystemModel>()
                .ForMember(x => x.TransactionProcessingType, z => z.MapFrom(y => y.TransactionProcessingType))
                .ForMember(x => x.StatusId, z => z.MapFrom(y => y.PersonStatusType != null ? y.PersonStatusType.Id : 0))
                ;


            Mapper.CreateMap<Institution, InstitutionGridModel>()
                .ForMember(x=>x.InstitutionType, opt => opt.MapFrom(src => src.InteropSystem.Description))
                .ForMember(x => x.Region, opt => opt.MapFrom(src => src.InstitutionAddress.Street.Region.Parent != null
                    ? src.InstitutionAddress.Street.Region.Parent.GetFullName()
                    : ""))
                .ForMember(x => x.FullAddress,
                    opt => opt.MapFrom(src => src.InstitutionAddress.GetFullAddress(true, true, false)))
                .ForMember(x => x.DataDeleted,
                    opt =>
                        opt.MapFrom(
                            src =>
                                (src.Deleted.HasValue
                                    ? src.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                                    : string.Empty)))
                ;


            Mapper.CreateMap<Institution, UpdateInstitutionModel>()
                .ForMember(x => x.InstitutionTypeId, z => z.MapFrom(y => y.InteropSystem.Id))
                .ForMember(x => x.AddressId, z => z.MapFrom(y => y.InstitutionAddress.Id));




            Mapper.CreateMap<Transaction, UpdateTransactionModel>();
            Mapper.CreateMap<Transaction, TransactionGridModel>()
                .ForMember(x => x.InteropSystem, opt => opt.MapFrom(src => src.InteropSystem.Description))
                .ForMember(x => x.Institution, opt => opt.MapFrom(src => (src.Institution != null ? src.Institution.Name : string.Empty)))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription(src.TransactionStatus)))
                .ForMember(x => x.DataDeleted,
                    opt =>
                        opt.MapFrom(
                            src =>
                                (src.Deleted.HasValue
                                    ? src.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                                    : string.Empty)))
                ;

        }
    }
}