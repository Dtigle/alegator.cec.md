using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Lookup;
using CEC.Web.SRV.Models.PollingStation;

namespace CEC.Web.SRV.Profiles
{
    public class LookupProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Lookup, LookupGridModel>()

                .ForMember(x => x.Id, y => y.MapFrom(x => x.Id))
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

                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(x => x.Description, opt => opt.MapFrom(src => src.Description))

                .Include<Street, StreetsGridModel>()
                .Include<RegionType, RegionTypesGridModel>()
                .Include<DocumentType, DocumentTypeGridModel>()
                .Include<PersonStatusType, PersonStatusGridModel>()
                .Include<Region, RegionTreeViewModel>();


            Mapper.CreateMap<RegionType, RegionTypesGridModel>();
            Mapper.CreateMap<DocumentType, DocumentTypeGridModel>();
            Mapper.CreateMap<ManagerType, UpdateLookupModel>();
            Mapper.CreateMap<PersonAddressType, UpdateLookupModel>();

            Mapper.CreateMap<Street, StreetsGridModel>()
                .ForMember(x => x.StreetType, opt => opt.MapFrom(src => src.StreetType.Name));

            Mapper.CreateMap<PersonStatusType, PersonStatusGridModel>();
            Mapper.CreateMap<PersonStatusType, UpdatePersonStatusModel>();
            Mapper.CreateMap<Gender, UpdateLookupModel>();
            Mapper.CreateMap<DocumentType, UpdateDocumentTypesModel>();
            Mapper.CreateMap<RegionType, UpdateRegionTypesModel>();
            Mapper.CreateMap<StreetType, UpdateLookupModel>();
            Mapper.CreateMap<StreetTypeCode, ClassifierGridModel>();
            Mapper.CreateMap<ElectionNumberListOrderBy, ElectionNumberListOrderByDto>();
            Mapper.CreateMap<RegionRow, RegionGridViewModel>()
                .ForMember(x => x.RegionType, opt => opt.MapFrom(src => src.RegionType.Name))
                .ForMember(x => x.CreatedById, z => z.MapFrom(y => y.CreatedBy))
                .ForMember(x => x.ModifiedById, z => z.MapFrom(y => y.ModifiedBy))
                .ForMember(x => x.DeletedById, z => z.MapFrom(y => y.DeletedBy))
                .ForMember(x => x.DataDeleted,
                z => z.MapFrom(y => y.Deleted.HasValue ? y.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
                .ForMember(x => x.DataModified,
                z => z.MapFrom(y => y.Modified.HasValue ? y.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
                .ForMember(x => x.DataCreated,
                z => z.MapFrom(y => y.Created.HasValue ? y.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty));
            Mapper.CreateMap<RegionRow, RegionTreeViewModel>()
                .ForMember(x => x.RegionType, opt => opt.MapFrom(src => src.RegionType.Name))
                .ForMember(x => x.CreatedById, z => z.MapFrom(y => y.CreatedBy))
                .ForMember(x => x.ModifiedById, z => z.MapFrom(y => y.ModifiedBy))
                .ForMember(x => x.DeletedById, z => z.MapFrom(y => y.DeletedBy))
                .ForMember(x => x.DataDeleted,
                z => z.MapFrom(y => y.Deleted.HasValue ? y.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
                .ForMember(x => x.DataModified,
                z => z.MapFrom(y => y.Modified.HasValue ? y.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
                .ForMember(x => x.DataCreated,
                z => z.MapFrom(y => y.Created.HasValue ? y.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty));

            //Mapper.CreateMap<Region, RegionTreeViewModel>()
            //    .ForMember(x => x.RegionType, opt => opt.MapFrom(src => src.RegionType.Name));

            Mapper.CreateMap<ElectionType, UpdateElectionTypeModel>()
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(x => x.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(x => x.ElectionCompetitorType, opt => opt.MapFrom(src => src.ElectionCompetitorType))
                .ForMember(x => x.ElectionRoundsNo, opt => opt.MapFrom(src => src.ElectionRoundsNo))
                .ForMember(x => x.ElectionArea, opt => opt.MapFrom(src => src.ElectionArea))
                .ForMember(x => x.AcceptResidenceDoc, opt => opt.MapFrom(src => src.AcceptResidenceDoc))
                .ForMember(x => x.AcceptVotingCert, opt => opt.MapFrom(src => src.AcceptVotingCert))
                .ForMember(x => x.CircumscriptionListId, opt => opt.MapFrom(src => (src.CircumscriptionList != null ? src.CircumscriptionList.Id : 0)))
                ;
            Mapper.CreateMap<ElectionType, ElectionTypeGridModel>()
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(x => x.ElectionCompetitorType, opt => opt.MapFrom(src => src.ElectionCompetitorType.GetEnumDescription()))
                .ForMember(x => x.ElectionRoundsNo, opt => opt.MapFrom(src => src.ElectionRoundsNo))
                .ForMember(x => x.ElectionArea, opt => opt.MapFrom(src => src.ElectionArea.GetEnumDescription()))
                .ForMember(x => x.AcceptResidenceDoc, opt => opt.MapFrom(src => src.AcceptResidenceDoc))
                .ForMember(x => x.AcceptVotingCert, opt => opt.MapFrom(src => src.AcceptVotingCert))
                .ForMember(x => x.CircumscriptionList, opt => opt.MapFrom(src => src.CircumscriptionList != null ? src.CircumscriptionList.Name : string.Empty))
                .ForMember(x => x.CreatedById, z => z.MapFrom(y => y.CreatedBy))
                .ForMember(x => x.ModifiedById, z => z.MapFrom(y => y.ModifiedBy))
                .ForMember(x => x.DeletedById, z => z.MapFrom(y => y.DeletedBy))
                .ForMember(x => x.DataDeleted,
                z => z.MapFrom(y => y.Deleted.HasValue ? y.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
                .ForMember(x => x.DataModified,
                z => z.MapFrom(y => y.Modified.HasValue ? y.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
                .ForMember(x => x.DataCreated,
                z => z.MapFrom(y => y.Created.HasValue ? y.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
            ;

            CreateMap<Circumscription, CircumscriptionGridModel>()
                .ForMember(x => x.Number, opt => opt.MapFrom(src => src.Number))
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.NameRo))
                .ForMember(x => x.CircumscriptionList, opt => opt.MapFrom(src => src.CircumscriptionList.Name))
                ;


            Mapper.CreateMap<Region, LocalitiesTreeView>()
                .ForMember(x => x.Name, z => z.MapFrom(y => y.GetFullName()))
                .ForMember(x => x.CreatedById, z => z.MapFrom(y => y.CreatedBy.UserName))
                .ForMember(x => x.ModifiedById, z => z.MapFrom(y => y.ModifiedBy.UserName))
                .ForMember(x => x.DeletedById, z => z.MapFrom(y => y.DeletedBy.UserName))
                .ForMember(x => x.DataDeleted,
                z => z.MapFrom(y => y.Deleted.HasValue ? y.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
                .ForMember(x => x.DataModified,
                z => z.MapFrom(y => y.Modified.HasValue ? y.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
                .ForMember(x => x.DataCreated,
                z => z.MapFrom(y => y.Created.HasValue ? y.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty));

            Mapper.CreateMap<Region, UpdateRegionModel>()
                .ForMember(x => x.RegionType, z => z.MapFrom(y => y.RegionType.Id))
                .ForMember(x => x.Parent, z => z.MapFrom(y => y.Parent.Id))
                .ForMember(x => x.Cuatm, z => z.MapFrom(y => y.StatisticIdentifier));

            Mapper.CreateMap<Region, UpdateCircumscriptionModel>()
                .ForMember(x => x.Name, z => z.MapFrom(y => y.GetFullName()))
                .ForMember(x => x.CircumscriptionNumber, z => z.MapFrom(y => y.Circumscription));

            Mapper.CreateMap<PublicAdministration, UpdatePublicAdministrationModel>()
                .ForMember(x => x.ManagerTypeId, z => z.MapFrom(y => y.ManagerType.Id))
                .ForMember(x => x.RegionId, z => z.MapFrom(y => y.Region.Id));

            Mapper.CreateMap<Street, UpdateStreetModel>()
                .ForMember(x => x.StreetTypeId, z => z.MapFrom(y => y.StreetType.Id));

            Mapper.CreateMap<PollingStation, UpdatePollingStationModel>()
                .ForMember(x => x.AddressId, z => z.MapFrom(y => y.PollingStationAddress.Id))
                .ForMember(x => x.StreetId, z => z.MapFrom(y => y.PollingStationAddress.Street.Id))
                .ForMember(x => x.VotersListOrderType, z => z.MapFrom(y => y.VotersListOrderType.Id));

            Mapper.CreateMap<PollingStation, UpdatePollingStationBaseModel>()
                .ForMember(x => x.VotersListOrderType, z => z.MapFrom(y => y.VotersListOrderType.Id));

            Mapper.CreateMap<StreetDto, StreetsGridModel>()
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

            .ForMember(x => x.CreatedById, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(x => x.ModifiedById, opt => opt.MapFrom(src => src.ModifiedBy))
            .ForMember(x => x.DeletedById, opt => opt.MapFrom(src => src.DeletedBy));
        }

    }
}