using AutoMapper;
using CEC.SRV.Domain;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Resources;
using RSP.CEC.WebClient.RspCecService;

namespace CEC.Web.SRV.Profiles
{
	public class RspCheckingProfile : Profile
	{
		protected override void Configure()
		{
			Mapper.CreateMap<PhysicalPersonRequestData, RspInfoModel>()
				.ForMember(x => x.FirstName, y => y.MapFrom(z => z.person.firstName))
				.ForMember(x => x.SecondName, y => y.MapFrom(z => z.person.secondName))
				.ForMember(x => x.LastName, y => y.MapFrom(z => z.person.lastName))
				.ForMember(x => x.CitizenRm, y => y.MapFrom(z => 
                    z.person.citizenRM.HasValue 
                    ? z.person.citizenRM.Value ? MUI.Yes : MUI.No
                    : string.Empty))
				.ForMember(x => x.Dead, y => y.MapFrom(z => z.person.dead.HasValue 
                    ? z.person.dead.Value ? MUI.Yes : MUI.No
                    : string.Empty))
				.ForMember(x => x.Idnp, y => y.MapFrom(z => z.person.idnp))
				.ForMember(x => x.Series, y => y.MapFrom(z => z.person.identDocument.series))
				.ForMember(x => x.Number, y => y.MapFrom(z => z.person.identDocument.number))
				.ForMember(x => x.Validity, y => y.MapFrom(z => z.person.identDocument.validity.HasValue
                    ? z.person.identDocument.validity.Value ? MUI.Yes : MUI.No
                    : string.Empty))
				.ForMember(x => x.IssueLocation, y => y.MapFrom(z => z.person.identDocument.issueLocation))
				.ForMember(x => x.BirthDate,
					y => y.MapFrom(z => z.person.birthDate.HasValue ? z.person.birthDate.Value.ToShortDateString() : string.Empty))
				.ForMember(x => x.IssueDate,
					y => y.MapFrom(z => z.person.identDocument.issueDate.HasValue
									        ? z.person.identDocument.issueDate.Value.ToShortDateString()
									        : string.Empty))
				.ForMember(x => x.ExpirationDate,
					y => y.MapFrom(z => z.person.identDocument.expirationDate.HasValue
									        ? z.person.identDocument.expirationDate.Value.ToShortDateString()
									        : string.Empty))
				.ForMember(x => x.Registration, y => y.MapFrom(z => z.person.registration));

			Mapper.CreateMap<RegistrationData, RegistrationDataModel>()
				.ForMember(x => x.Region, y => y.MapFrom(z => z.address.region))
				.ForMember(x => x.Locality, y => y.MapFrom(z => z.address.locality))
				.ForMember(x => x.AdministrativCode, y => y.MapFrom(z => z.address.administrativCode))
				.ForMember(x => x.Street, y => y.MapFrom(z => z.address.street))
				.ForMember(x => x.StreetCode, y => y.MapFrom(z => z.address.streetCode))
				.ForMember(x => x.RegTypeCode, y => y.MapFrom(z => z.regTypeCode))
				.ForMember(x => x.House, y => y.MapFrom(z => z.address.house))
				.ForMember(x => x.Block, y => y.MapFrom(z => z.address.block))
				.ForMember(x => x.Flat, y => y.MapFrom(z => z.address.flat))
				.ForMember(x => x.RegDate, y => y.MapFrom(z => z.regDate.HasValue ? z.regDate.Value.ToShortDateString() : string.Empty))
				.ForMember(x => x.ExpirationDateAddress, y => y.MapFrom(z => z.expirationDate.HasValue ? z.expirationDate.Value.ToShortDateString() : string.Empty));
		}
	}
}