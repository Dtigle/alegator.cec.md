using AutoMapper;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.ViewItem;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Address;
using CEC.Web.SRV.Models.Conflict;
using CEC.Web.SRV.Models.Grid;

namespace CEC.Web.SRV.Profiles
{
    public class ConflictProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<RspConflictData, ConflictGridModel>()
                .ForMember(x => x.DateOfBirth, y => y.MapFrom(z => z.Birthdate.ToShortDateString()))
                .ForMember(x => x.Gender, y => y.MapFrom(z => z.SexCode == "2" ? "F" : "M"))
                .ForMember(x => x.DocType, y => y.ResolveUsing(z =>
                {
                    switch (z.Doctypecode)
                    {
                        case 5:
                            return "Buletin";
                        case 3:
                            return "Pașaport";
                        case 71:
                            return "F-9";
                        case 60:
                            return "Sovietic";
                        default:
                            return string.Empty;
                    }
                }))
                .ForMember(x => x.Series, y => y.MapFrom(z => z.Series))
                .ForMember(x => x.Number, y => y.MapFrom(z => z.Number))
                .ForMember(x => x.IssueDate,
                    y => y.MapFrom(z => z.Issuedate.HasValue ? z.Issuedate.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.ExpirationDate,
                    y =>
                        y.MapFrom(
                            z => z.Expirationdate.HasValue ? z.Expirationdate.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.AdministrativeCode, y => y.MapFrom(z => z.Administrativecode))
                .ForMember(x => x.Status, y => y.MapFrom(z => z.Status.GetEnumDescription()))
                .ForMember(x => x.Source, y => y.MapFrom(z => z.Source.GetEnumDescription()))
                .ForMember(x => x.DataCreated, y => y.MapFrom(z => z.Created.LocalDateTime.ToString()))
                .ForMember(x => x.StatusDate,
                    y =>
                        y.MapFrom(
                            z => z.StatusDate.HasValue ? z.StatusDate.Value.LocalDateTime.ToString() : string.Empty))
                .ForMember(x => x.RegionId, y => y.MapFrom(z => z.SrvRegion.Id))
                ;
            Mapper.CreateMap<RspConflictDataAdmin, ConflictGridModel>()
                .ForMember(x => x.DateOfBirth, y => y.MapFrom(z => z.Birthdate.ToShortDateString()))
                .ForMember(x => x.Gender, y => y.MapFrom(z => z.SexCode == "2" ? "F" : "M"))
                .ForMember(x => x.DocType, y => y.ResolveUsing(z =>
                {
                    switch (z.Doctypecode)
                    {
                        case 5:
                            return "Buletin";
                        case 3:
                            return "Pașaport";
                        case 71:
                            return "F-9";
                        case 60:
                            return "Sovietic";
                        default:
                            return string.Empty;
                    }
                }))
                .ForMember(x => x.Series, y => y.MapFrom(z => z.Series))
                .ForMember(x => x.Number, y => y.MapFrom(z => z.Number))
                .ForMember(x => x.IssueDate,
                    y => y.MapFrom(z => z.Issuedate.HasValue ? z.Issuedate.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.ExpirationDate,
                    y =>
                        y.MapFrom(
                            z => z.Expirationdate.HasValue ? z.Expirationdate.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.AdministrativeCode, y => y.MapFrom(z => z.Administrativecode))
                .ForMember(x => x.Status, y => y.MapFrom(z => z.Status.GetEnumDescription()))
                .ForMember(x => x.Source, y => y.MapFrom(z => z.Source.GetEnumDescription()))
                .ForMember(x => x.DataCreated, y => y.MapFrom(z => z.Created.LocalDateTime.ToString()))
                .ForMember(x => x.StatusDate,
                    y =>
                        y.MapFrom(
                            z => z.StatusDate.HasValue ? z.StatusDate.Value.LocalDateTime.ToString() : string.Empty))
                .ForMember(x => x.RegionId, y => y.MapFrom(z => z.SrvRegion.Id))
                ;
            Mapper.CreateMap<VoterConflictDataDto, VoterConflictModel>()
                .ForMember(x => x.DateOfBirth, y => y.MapFrom(z => z.DateOfBirth.ToShortDateString()))
                .ForMember(x => x.DocIssueBy, y => y.MapFrom(z => z.DocIssueBy))
                .ForMember(x => x.DocIssueDate,
                    y =>
                        y.MapFrom(z => z.DocIssueDate.HasValue ? z.DocIssueDate.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.DocValidBy,
                    y => y.MapFrom(z => z.DocValidBy.HasValue ? z.DocValidBy.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.DocNumber, y => y.MapFrom(z => z.DocNumber))
                .ForMember(x => x.DocSeria, y => y.MapFrom(z => z.DocSeria))
                ;
            Mapper.CreateMap<RspModificationData, PeopleConflictModel>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Birthdate, y => y.MapFrom(z => z.Birthdate.ToShortDateString()))
                .ForMember(x => x.Gender, y => y.MapFrom(z => z.GetSexCode()))
                .ForMember(x => x.PersonStatus, y => y.MapFrom(z => z.GetPersonStatus()))
                .ForMember(x => x.DocType, y => y.MapFrom(z => z.DocumentTypeCode))
                .ForMember(x => x.Series, y => y.MapFrom(z => z.Seria))
                .ForMember(x => x.Number, y => y.MapFrom(z => z.Number))
                .ForMember(x => x.IssueDate,
                    y => y.MapFrom(z => z.IssuedDate.HasValue ? z.IssuedDate.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.ExpirationDate,
                    y => y.MapFrom(z => z.ValidBydate.HasValue ? z.ValidBydate.Value.ToShortDateString() : string.Empty))
                .ForMember(x => x.Validity, y => y.MapFrom(z => z.Validity ? "Da" : "Nu"))
                .ForMember(x => x.Source, y => y.MapFrom(z => z.Source.GetEnumDescription()))
                .ForMember(x => x.Status, y => y.MapFrom(z => z.Status.GetEnumDescription()))
                .ForMember(x => x.Created, y => y.MapFrom(z => z.Created.LocalDateTime.ToString()))
                .ForMember(x => x.StatusDate, y => y.MapFrom(z =>
                    z.StatusDate.HasValue ? z.StatusDate.Value.LocalDateTime.ToString() : string.Empty))
                ;

            Mapper.CreateMap<AddressBaseDto, AddressRSVGridModel>()
                .ForMember(x => x.StreetName, opt => opt.MapFrom(src => src.GetFullName(true)))
                .ForMember(x => x.PollingStation, opt => opt.MapFrom(src => src.PollingStation))
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
                ;
            Mapper.CreateMap<RspModificationData, VoterConflictGridModel>()
                .ForMember(x => x.ConflictDate, y => y.MapFrom(z => z.Created.DateTime.ToString()))
                .ForMember(x => x.ConflictType,
                    y => y.MapFrom(z => VoterConflictGridModel.GetConflictTypeString(z.StatusConflictCode)))
                .ForMember(x => x.Status, y => y.MapFrom(z => z.IsResolved() ? "Rezolvat" : "Activ"))
                .ForMember(x => x.StatusMessage, y => y.MapFrom(z => z.StatusMessage));


            Mapper.CreateMap<ConflictShare, ConflictShareViewModel>()
                .ForMember(x => x.SourceRegionId, y => y.MapFrom(z => z.Source.Id))
                .ForMember(x => x.SourceRegionName, y => y.MapFrom(z => z.Source.GetFullName()))
                .ForMember(x => x.DestinationRegionId, y => y.MapFrom(z => z.Destination.Id))
                .ForMember(x => x.DestinationRegionName, y => y.MapFrom(z => z.Destination.GetFullName()))
                .ForMember(x => x.Reason, y => y.MapFrom(z => z.Reason.Name))
                .ForMember(x => x.Created, y => y.MapFrom(z => z.Created.Value.LocalDateTime.ToString()))
                .ForMember(x => x.CreatedBy, y => y.MapFrom(z => z.CreatedBy.UserName))
                ;


            Mapper.CreateMap<ConflictViewItem, ConflictGridModel>()
               .ForMember(x => x.DateOfBirth, y => y.MapFrom(z => z.Birthdate.ToShortDateString()))
               .ForMember(x => x.Gender, y => y.MapFrom(z => z.SexCode == "2" ? "F" : "M"))
               .ForMember(x => x.DocType, y => y.ResolveUsing(z =>
               {
                   switch (z.Doctypecode)
                   {
                       case 5:
                           return "Buletin";
                       case 3:
                           return "Pașaport";
                       case 71:
                           return "F-9";
                       case 60:
                           return "Sovietic";
                       default:
                           return string.Empty;
                   }
               }))
               .ForMember(x => x.IssueDate,
                   y => y.MapFrom(z => z.Issuedate.HasValue ? z.Issuedate.Value.ToShortDateString() : string.Empty))
               .ForMember(x => x.ExpirationDate,
                   y =>
                       y.MapFrom(
                           z => z.Expirationdate.HasValue ? z.Expirationdate.Value.ToShortDateString() : string.Empty))
               .ForMember(x => x.Status, y => y.MapFrom(z => z.Status.GetEnumDescription()))
               .ForMember(x => x.Source, y => y.MapFrom(z => z.Source.GetEnumDescription()))
               .ForMember(x => x.DataCreated, y => y.MapFrom(z => z.Created.LocalDateTime.ToString()))
               .ForMember(x => x.StatusDate,
                   y =>
                       y.MapFrom(
                           z => z.StatusDate.HasValue ? z.StatusDate.Value.LocalDateTime.ToString() : string.Empty))

               ;

            Mapper.CreateMap<AddressWithoutPollingStation, AddressWithoutPollingStationGridModel>();
        }
    }
}