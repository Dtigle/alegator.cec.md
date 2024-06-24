using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.ViewItem;
using CEC.Web.SRV.Models.Address;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Models.Notification;
using CEC.Web.SRV.Models.PollingStation;
using CEC.Web.SRV.Models.Statistics;
using CEC.Web.SRV.Models.Voters;

namespace CEC.Web.SRV.Profiles
{
    public class SrvGridModelsProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<SRVBaseEntity, JqGridSoft>()

                .ForMember(x => x.Id, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.DataCreated,
                    opt =>
                        opt.MapFrom(
                            src => (src.Created.HasValue ? src.Created.Value.LocalDateTime.ToString() : string.Empty)))
                .ForMember(x => x.DataModified,
                    opt =>
                        opt.MapFrom(
                            src => (src.Modified.HasValue ? src.Modified.Value.LocalDateTime.ToString() : string.Empty)))
                .ForMember(x => x.DataDeleted,
                    opt =>
                        opt.MapFrom(
                            src => (src.Deleted.HasValue ? src.Deleted.Value.LocalDateTime.ToString() : string.Empty)))

                .ForMember(x => x.CreatedById,
                    opt => opt.MapFrom(src => (src.CreatedBy != null ? src.CreatedBy.UserName : string.Empty)))
                .ForMember(x => x.ModifiedById,
                    opt => opt.MapFrom(src => (src.ModifiedBy != null ? src.ModifiedBy.UserName : string.Empty)))
                .ForMember(x => x.DeletedById,
                    opt => opt.MapFrom(src => (src.DeletedBy != null ? src.DeletedBy.UserName : string.Empty)))
                .Include<Address, AddressGridModel>()
                //.Include<Address, AddressApartmentGridModel>()
                //.Include<Address, AddressAdministrativeGridModel>()
				.Include<Address, AddressBuildingsGridModel>()
                .Include<PollingStation, PollingStationGridModel>()
                .Include<StayStatement, StayStatementsGridModel>()
                .Include<Person, VotersGridModel>()
                .Include<VoterViewItem, VotersGridModel>()
                .Include<Person, SearchVotersGridModel>()
                .Include<NotificationReceiver, NotificationGridModel>();

            Mapper.CreateMap<Address, AddressGridModel>()
                .ForMember(x => x.StreetName, opt => opt.MapFrom(src => src.Street.GetFullName(true)))
				.Include<Address, AddressAdministrativeGridModel>()
				.Include<Address, AddressApartmentGridModel>();

            Mapper.CreateMap<VoterViewItem, VotersGridModel>()
                .ForMember(x => x.DataOfBirth, y => y.MapFrom(z => z.DateOfBirth != null ? z.DateOfBirth.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.AddressExpirationDate, y => y.MapFrom(z => z.AddressExpirationDate != null ? z.AddressExpirationDate.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.electionListNr, y => y.MapFrom(z => z.electionListNr))
                ;

            Mapper.CreateMap<NotificationReceiver, NotificationGridModel>()
		        .ForMember(x => x.Message, opt => opt.MapFrom(src => src.Notification.Message))
		        //.ForMember(x => x.CreateDate, opt => opt.MapFrom(src => src.Notification.CreateDate.ToShortDateString()))
		        .ForMember(x => x.CreateDate, opt => opt.MapFrom(src => src.Notification.CreateDate.ToLocalTime()))
		        .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.CreatedBy.UserName))
		        .ForMember(x => x.NotificationType, opt => opt.MapFrom(src => src.Notification.Event.EntityType));

			//Mapper.CreateMap<Address, AddressAdministrativeGridModel>()
			//	.ForMember(x => x.GeoLocation, opt => opt.MapFrom(src => src.GeoLocation != null));

			//Mapper.CreateMap<Address, AddressApartmentGridModel>()
			//	.ForMember(x => x.PollingStation, opt => opt.MapFrom(src => src.PollingStation.GetFullNumber()));

			Mapper.CreateMap<Address, AddressBuildingsGridModel>()
				.ForMember(x => x.PollingStation, opt => opt.MapFrom(src => src.PollingStation.FullNumber))
			    .ForMember(x => x.GeoLocation, opt => opt.MapFrom(src => src.GeoLocation != null));

			Mapper.CreateMap<StayStatement, StayStatementsGridModel>()
			   .ForMember(x => x.StayStatementId, opt => opt.MapFrom(src => src.Id))
			   .ForMember(x => x.StayStatementIdnp, opt => opt.MapFrom(src => src.Person.Idnp))
			   .ForMember(x => x.PersonName, opt => opt.MapFrom(src => src.Person.FullName))
			   .ForMember(x => x.PersonDateOfBirth, opt => opt.MapFrom(src => src.Person.DateOfBirth.ToShortDateString()))
			   .ForMember(x => x.BaseAddressInfo, opt => opt.MapFrom(src => src.BaseAddress.GetFullPersonAddress(true)))
			   .ForMember(x => x.DeclaredStayAddressInfo, opt => opt.MapFrom(src => src.DeclaredStayAddress.GetFullPersonAddress(true)))
			   .ForMember(x => x.ElectionInfo, opt => opt.MapFrom(src => src.ElectionInstance.NameRo));

	        Mapper.CreateMap<PollingStation, PollingStationGridModel>()
		        .ForMember(x => x.FullAddress,opt => opt.MapFrom(src => src.PollingStationAddress.GetFullAddress(true, true, false)))
		        .ForMember(x => x.TotalAddress, opt => opt.MapFrom(src => src.Addresses.Count))
		        .ForMember(x => x.CircumscriptionNumber, opt => opt.MapFrom(src => src.Region.GetCircumscription()));


            Mapper.CreateMap<VoterRow, VotersGridModel>()
                .ForMember(x => x.DataOfBirth, y => y.MapFrom(z => z.BirthDate.ToShortDateString()))
                .ForMember(x => x.ApNumber, y => y.MapFrom(z => z.ApartmentNumber))
                .ForMember(x => x.DocumentNumber, y => y.MapFrom(z => z.DocumentNumber))
                .ForMember(x => x.Address, y => y.MapFrom(z => z.Address))
                .ForMember(x => x.ApSuffix, y => y.MapFrom(z => z.ApartmentSuffix))
                .ForMember(x => x.Age, y => y.MapFrom(z => z.Age))
                .ForMember(x => x.RegionHasStreets, y => y.MapFrom(z => z.RegionHasStreets))
                .ForMember(x => x.electionListNr, y => y.MapFrom(z => z.electionListNr))
                .ForMember(x => x.DataCreated, y => y.MapFrom(z => z.Created.HasValue ? z.Created.Value.LocalDateTime.ToString() : string.Empty))
                .ForMember(x => x.DataModified, y => y.MapFrom(z => z.Modified.HasValue ? z.Modified.Value.LocalDateTime.ToString() : string.Empty))
                .ForMember(x => x.DataDeleted, y => y.MapFrom(z => z.Deleted.HasValue ? z.Deleted.Value.LocalDateTime.ToString() : string.Empty));
            
            Mapper.CreateMap<Person, VotersGridModel>()
                .ForMember(x => x.Address, opt => opt.ResolveUsing(src =>
                {
                    var personAddress = src.EligibleAddress;
                    if (personAddress != null)
                    {
                        return personAddress.Address.GetFullAddress(true, showpollingStation: true);
                    }

                    return string.Empty;
                }))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => src.CurrentStatus.StatusType.Name))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.Gender.Name))
                .ForMember(x => x.ApNumber, opt => opt.ResolveUsing(src =>
                {
                    var personAddress = src.EligibleAddress;
                    if (personAddress != null)
                    {
                        return personAddress.ApNumber;
                    }

                    return null;
                }))
                .ForMember(x => x.ApSuffix, opt => opt.ResolveUsing(src =>
                {
                    var personAddress = src.EligibleAddress;
                    if (personAddress != null)
                    {
                        return personAddress.ApSuffix;
                    }

                    return string.Empty;
                }))
                .ForMember(x => x.AddressType, opt => opt.MapFrom(src => src.EligibleAddress.PersonAddressType.Name))
                //.ForMember(x => x.DocumentSeries, opt => opt.MapFrom(src => src.Document.Seria))
                .ForMember(x => x.electionListNr, opt => opt.MapFrom(src => src.ElectionListNr))
                .ForMember(x => x.DocumentNumber, opt => opt.MapFrom(src => src.Document.Number))
                .ForMember(x => x.DataOfBirth, opt => opt.MapFrom(src => (src.DateOfBirth.ToShortDateString())));

            Mapper.CreateMap<Person, SearchVotersGridModel>()
                .ForMember(x => x.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(x => x.DataOfBirth, opt => opt.MapFrom(src => (src.DateOfBirth.ToShortDateString())))
                .ForMember(x => x.Document, opt => opt.MapFrom(src => src.Document.DocumentNumber))
                .ForMember(x => x.Address,
                    opt => opt.MapFrom(src => src.EligibleAddress.Address.GetFullAddress(true, true, true)))
                .ForMember(x => x.AddressType, opt => opt.MapFrom(src => src.EligibleAddress.PersonAddressType.Name))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => src.CurrentStatus.StatusType.Name));



            Mapper.CreateMap<PollingStationStatistics, StatisticsPollingStationGridModel>();


			Mapper.CreateMap<AddressDto, AddressBuildingsGridModel>()
				 .ForMember(x => x.StreetName, opt => opt.MapFrom(src => src.GetFullName(true)))
				 .ForMember(x => x.PollingStation, opt => opt.MapFrom(src => src.PollingStation))
				 .ForMember(x => x.GeoLocation, opt => opt.MapFrom(src => src.GeoLocation != null))
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
					opt => opt.MapFrom(src => src.CreatedBy ))
				.ForMember(x => x.ModifiedById,
					opt => opt.MapFrom(src => src.ModifiedBy ))
				.ForMember(x => x.DeletedById,
					opt => opt.MapFrom(src => src.DeletedBy ));

			Mapper.CreateMap<PollingStationDto, PollingStationGridModel>()
				 .ForMember(x => x.CircumscriptionNumber, opt => opt.MapFrom(src => src.OwingCircumscription))
				 .ForMember(x => x.Number, opt => opt.MapFrom(src => src.Number))
				 .ForMember(x => x.Location, opt => opt.MapFrom(src => src.Location))
				 .ForMember(x => x.SaiseId, opt => opt.MapFrom(src => src.SaiseId))
				 .ForMember(x => x.FullAddress, opt => opt.MapFrom(src => src.FullAddress))
				 .ForMember(x => x.TotalAddress, opt => opt.MapFrom(src => src.TotalAddress))
				 .ForMember(x => x.VotersListOrderTypes, opt => opt.MapFrom(src => src.VotersListOrderTypes))
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

				.ForMember(x => x.CreatedById,opt => opt.MapFrom(src => src.CreatedBy))
				.ForMember(x => x.ModifiedById,opt => opt.MapFrom(src => src.ModifiedBy))
				.ForMember(x => x.DeletedById,opt => opt.MapFrom(src => src.DeletedBy))
                .ForMember(x => x.OrderType, opt => opt.MapFrom(src => src.OrderType ?? "Implicit"));
        }
    }
}