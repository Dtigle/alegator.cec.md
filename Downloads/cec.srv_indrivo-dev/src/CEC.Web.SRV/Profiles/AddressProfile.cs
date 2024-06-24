using AutoMapper;
using CEC.SRV.Domain;
using CEC.Web.SRV.Models.Address;
using System.Globalization;

namespace CEC.Web.SRV.Profiles
{
    public class AddressProfile : Profile
	{
        protected override void Configure()
        {
			//Mapper.CreateMap<Address, UpdateAdministrativeAddressModel>()
			//	.ForMember(x => x.StreetId, y => y.MapFrom(z => z.Street.Id));

			//Mapper.CreateMap<Address, UpdateApartmentAddressModel>()
			//	.ForMember(x => x.StreetId, y => y.MapFrom(z => z.Street.Id))
			//	.ForMember(x => x.PollingStationId, y => y.MapFrom(z => z.PollingStation.Id));

            Mapper.CreateMap<PersonAddress, PersonFullAdressGridModel>()
                .ForMember(x => x.Created, y => y.MapFrom(z => z.Created.Value.DateTime.ToString()))
                .ForMember(x => x.Modified, y => y.MapFrom(z => z.Modified.HasValue ? z.Modified.Value.DateTime.ToString() : string.Empty))
                .ForMember(x => x.DataModified, y => y.MapFrom(z => z.Modified.HasValue ? z.Modified.Value.DateTime.ToString() : string.Empty))
                .ForMember(x => x.DateOfExpiration, y => y.MapFrom(z => z.DateOfExpiration.HasValue
                                    ? z.DateOfExpiration.Value.ToString(CultureInfo.CurrentCulture)
                                    : string.Empty))
                .ForMember(x => x.AddressType, y => y.MapFrom(z => z.PersonAddressType.Name))
                .ForMember(x => x.Locality, y => y.MapFrom(z => z.Address.Street.Region.GetFullName()))
                .ForMember(x => x.Region, y => y.MapFrom(z => z.Address.Street.Region.Parent != null ? z.Address.Street.Region.Parent.GetFullName(): null))
                .ForMember(x => x.Street, y => y.MapFrom(z => z.Address.Street.Name))
                .ForMember(x => x.BlockNumber, y => y.MapFrom(z => z.Address.HouseNumber));

	        Mapper.CreateMap<Address, UpdateBuildingAddressModel>()
		        .ForMember(x => x.StreetId, y => y.MapFrom(z => z.Street.Id))
		        .ForMember(x => x.PollingStationId, y => y.MapFrom(z => z.PollingStation.Id));

            Mapper.CreateMap<RspAddressMapping, RspAddressMappingGridModel>()
                .ForMember(x => x.SrvAddress, y => y.MapFrom(z => z.SrvFullAddress))
                .ForMember(x => x.SrvRegion, y => y.MapFrom(z => z.SrvRegion.FullyQualifiedName))
                .ForMember(x => x.RspStreet, y => y.MapFrom(z => z.RspStreetName))
                .ForMember(x => x.RspRegion, y => y.MapFrom(z => z.RspRegion.FullyQualifiedName));
        }
	}
}