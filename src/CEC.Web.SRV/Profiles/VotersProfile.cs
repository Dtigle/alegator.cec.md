using AutoMapper;
using CEC.SRV.Domain;
using CEC.Web.SRV.Models.Voters;

namespace CEC.Web.SRV.Profiles
{
    public class VotersProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Person, UpdateVotersStatusModel>()
                .ForMember(x => x.CurrentStatus, y => y.MapFrom(z => z.CurrentStatus.StatusType.Name))
                .ForMember(x => x.CurrentConfirmation, y => y.MapFrom(z => z.CurrentStatus.Confirmation))
                .ForMember(x => x.PersonInfo, y => y.MapFrom(z => z));

            Mapper.CreateMap<Person, PersonInfo>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Address, y => y.MapFrom(z => z.EligibleAddress.GetFullPersonAddress(true)))
                .ForMember(x => x.FullName, y => y.MapFrom(z => z.FullName))
                .ForMember(x => x.Idnp, y => y.MapFrom(z => z.Idnp))
                .ForMember(x => x.PrimaryDocument, y => y.MapFrom(z => z.Document.DocumentNumber));

            Mapper.CreateMap<PersonDocument, VoterDocumentGridModel>()
                .ForMember(x => x.IssuedDate, y => y.MapFrom(z => z.IssuedDate.HasValue == true ? z.IssuedDate.Value.Date.ToString() : ""))
                .ForMember(x => x.ValidBy, y => y.MapFrom(z => z.ValidBy.HasValue == true ? z.ValidBy.Value.Date.ToString() : ""));

            Mapper.CreateMap<PollingStation, VoterPollingStationHistoryGridModel>()
                .ForMember(x => x.FullNumber, y => y.MapFrom(z => z.FullNumber))
                .ForMember(x => x.Region, y => y.MapFrom(z => z.Region.GetFullName()))
                .ForMember(x => x.PollingStationAddress, y => y.MapFrom(z => z.PollingStationAddress.GetFullAddress(true, true, true)))
                .ForMember(x => x.PollingStationType, y => y.MapFrom(z => z.PollingStationType == PollingStationTypes.Normal ? "Normal" : "Extrateritorial"));


        }
    }
}