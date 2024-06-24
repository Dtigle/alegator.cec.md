using System;
using System.Linq;
using System.Globalization;
using AutoMapper;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.History;

namespace CEC.Web.SRV.Profiles
{
    public class HistoryProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Lookup, LookupHistoryGridModel>()
                .ForMember(x => x.Id, y => y.MapFrom(x => string.Format("{0}_{1:N}", x.Id, Guid.NewGuid())))
                .ForMember(x => x.DataCreated, opt => opt.MapFrom(
                    src => (src.Created.HasValue
                        ? src.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                        : string.Empty)))
                .ForMember(x => x.DataModified, opt => opt.MapFrom(
                    src => (src.Modified.HasValue
                        ? src.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                        : string.Empty)))
                .ForMember(x => x.DataDeleted, opt => opt.MapFrom(
                    src => (src.Deleted.HasValue
                        ? src.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
                        : string.Empty)))
                .ForMember(x => x.CreatedById, opt => opt.MapFrom(
                    src => (src.CreatedBy != null ? src.CreatedBy.UserName : string.Empty)))
                    //src => (src.CreatedBy != null ? src.CreatedBy.Id : string.Empty)))
                .ForMember(x => x.ModifiedById, opt => opt.MapFrom(
                    src => (src.ModifiedBy != null ? src.ModifiedBy.UserName : string.Empty)))
                    //src => (src.ModifiedBy != null ? src.ModifiedBy.Id : string.Empty)))
                .ForMember(x => x.DeletedById, opt => opt.MapFrom(
                    src => (src.DeletedBy != null ? src.DeletedBy.UserName : string.Empty)))
                    //src => (src.DeletedBy != null ? src.DeletedBy.Id : string.Empty)))

                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(x => x.Description, opt => opt.MapFrom(src => src.Description))
                .Include<PersonStatusType, PersonStatusHistoryGridModel>()
                .Include<DocumentType, DocumentTypeHistoryGridModel>()
                .Include<RegionType, RegionTypeHistoryGridModel>()
                .Include<Region, RegionHistoryGridModel>()
                .Include<Street, StreetsHistoryGridModel>();

            Mapper.CreateMap<PersonStatusType, PersonStatusHistoryGridModel>();
            Mapper.CreateMap<DocumentType, DocumentTypeHistoryGridModel>();
            Mapper.CreateMap<RegionType, RegionTypeHistoryGridModel>();

            Mapper.CreateMap<Region, RegionHistoryGridModel>()
                .ForMember(x => x.RegionType, opt => opt.MapFrom(src => src.RegionType.Id));

            Mapper.CreateMap<Street, StreetsHistoryGridModel>()
                //.ForMember(x => x.StreetType, opt => opt.MapFrom(src => src.StreetType.Name));
                .ForMember(x => x.StreetType, opt => opt.MapFrom(src => src.StreetType.Id));

	        Mapper.CreateMap<SRVBaseEntity, HistoryGridRow>()
		        .ForMember(x => x.Id, y => y.MapFrom(x => string.Format("{0}_{1:N}", x.Id, Guid.NewGuid())))
		        //.ForMember(x => x.Id, y => y.MapFrom(x => x.Id))
		        .ForMember(x => x.DataCreated, opt => opt.MapFrom(
			        src =>
				        (src.Created.HasValue
					        ? src.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
					        : string.Empty)))
		        .ForMember(x => x.DataModified, opt => opt.MapFrom(
			        src =>
				        (src.Modified.HasValue
					        ? src.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
					        : string.Empty)))
		        .ForMember(x => x.DataDeleted, opt => opt.MapFrom(
			        src =>
				        (src.Deleted.HasValue
					        ? src.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture)
					        : string.Empty)))

		        .ForMember(x => x.CreatedById, opt => opt.MapFrom(
			        src => (src.CreatedBy != null ? src.CreatedBy.UserName : string.Empty)))
			        //src => (src.CreatedBy != null ? src.CreatedBy.Id : string.Empty)))
		        .ForMember(x => x.ModifiedById, opt => opt.MapFrom(
			        src => (src.ModifiedBy != null ? src.ModifiedBy.UserName : string.Empty)))
			        //src => (src.ModifiedBy != null ? src.ModifiedBy.Id : string.Empty)))
		        .ForMember(x => x.DeletedById, opt => opt.MapFrom(
			        src => (src.DeletedBy != null ? src.DeletedBy.UserName : string.Empty)))
			        //src => (src.DeletedBy != null ? src.DeletedBy.Id : string.Empty)))

		        .Include<PollingStation, PollingStationHistoryGridModel>()
		        .Include<Address, AddressHistoryGridModel>()
		        .Include<Election, ElectionHistoryGridModel>()
                .Include<ConfigurationSetting, ConfigurationSettingHistoryGridModel>();

            Mapper.CreateMap<PollingStation, PollingStationHistoryGridModel>()
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.PollingStationAddress.Id));

	        Mapper.CreateMap<Address, AddressHistoryGridModel>()
				//.ForMember(x => x.PollingStation, opt => opt.MapFrom(src => src.PollingStation.GetFullNumber()))
				.ForMember(x => x.PollingStation, opt => opt.MapFrom(src => src.PollingStation.Id))
				//.ForMember(x => x.StreetName, opt => opt.MapFrom(src => src.Street.GetFullName(true)))
				.ForMember(x => x.StreetName, opt => opt.MapFrom(src => src.Street.Id))
				.ForMember(x => x.HouseNumber, opt => opt.MapFrom(src => src.HouseNumber))
				.ForMember(x => x.Suffix, opt => opt.MapFrom(src => src.Suffix));


            Mapper.CreateMap<Election, ElectionHistoryGridModel>()
                .ForMember(x => x.ElectionType, z => z.MapFrom(y => y.ElectionType.Name))
                .ForMember(x => x.ElectionType, z => z.MapFrom(y => y.ElectionType.Id))
                .ForMember(x => x.ElectionDate, z => z.MapFrom(y => y.ElectionRounds.LastOrDefault().ElectionDate.ToShortDateString()))
                //.ForMember(x => x.SaiseId, z => z.MapFrom(y => y.SaiseId))
                .ForMember(x => x.Comments, z => z.MapFrom(y => y.Description));

            Mapper.CreateMap<ConfigurationSetting, ConfigurationSettingHistoryGridModel>()
                .ForMember(x => x.Name, z => z.MapFrom(y => y.Name))
                .ForMember(x => x.Value, z => z.MapFrom(y => y.Value))
                .ForMember(x => x.ApplicationName, z => z.MapFrom(y => y.ApplicationName));
        }
    }
}