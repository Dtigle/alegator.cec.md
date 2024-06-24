using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Models.VoterCertificate;

namespace CEC.SAISE.EDayModule.Profiles
{
    public class VoterCertificatProfile :Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<VoterCertificatDto, VoterCertificateGridModel>()
               .ForMember(c => c.CerificatID, op => op.MapFrom(v => v.Id))
               .ForMember(c => c.Election, op => op.MapFrom(v => v.Election.ToShortDateString()))
               .ForMember(c => c.Idnp, op => op.MapFrom(v => v.Idnp.ToString("0000000000000")))
               .ForMember(c => c.ReleaseDate, op => op.MapFrom(v => v.ReleaseDate.Value.ToShortDateString()));


            Mapper.CreateMap<AssignedVoter, VoterGridViewModel>()
                .ForMember(c => c.Id, op => op.MapFrom(v => v.Id))
                .ForMember(c => c.Idnp, op => op.MapFrom(v => v.Voter.Idnp.ToString("0000000000000")))
                .ForMember(c => c.FullName, op => op.MapFrom(v => v.Voter.NameRo + " " + v.Voter.LastNameRo))
                .ForMember(c => c.Address, op => op.MapFrom(v => v.Region.RegionType.Name + " " + v.Region.Name + " " + v.Voter.GetAddress()))
                .ForMember(c => c.Document, op => op.MapFrom(v => v.Voter.DocumentNumber))
                .ForMember(c => c.DataOfBirth, op => op.MapFrom(v => v.Voter.DateOfBirth != null ? v.Voter.DateOfBirth.ToShortDateString() : string.Empty))
                .ForMember(c => c.Certificat, op => op.MapFrom(v => v.VoterCertificats.FirstOrDefault(x => x.DeletedDate == null) != null ? true : false));

            Mapper.CreateMap<VoterCertificatDto, CertificatModel>()
                .ForMember(c => c.Id, op => op.MapFrom(v => v.Id))
                .ForMember(c => c.IDNP, op => op.MapFrom(v => v.Idnp.ToString("0000000000000")))
                .ForMember(c => c.FullName, op => op.MapFrom(v => v.FirstName + " " + v.LastName))
                .ForMember(c => c.Adres, op => op.MapFrom(v => v.Adress))
                .ForMember(c => c.BirthDate, op => op.MapFrom(v => v.BirthDate))
                .ForMember(c => c.DocumentNumber, op => op.MapFrom(v => v.DocumentNr))
                .ForMember(c => c.DocumentData, op => op.MapFrom(v => v.DocumentData))
                .ForMember(c => c.DocumentExpireData, op => op.MapFrom(v => v.DocumentExpireData))
                .ForMember(c => c.TypeElection, op => op.MapFrom(v => v.ElectionType)) 
                .ForMember(c => c.Circumscription, op => op.MapFrom(v => v.PollingStation))
                .ForMember(c => c.ElectionOffice, op => op.MapFrom(v => v.PollingStationRegion))  
                .ForMember(c => c.ReleaseDate, op => op.MapFrom(v => v.ReleaseDate))
                .ForMember(c => c.ElectionDate, op => op.MapFrom(v => v.Election)); 
        }
    }
}