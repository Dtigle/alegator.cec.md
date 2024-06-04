using AutoMapper;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.Web.SRV.Models.Export;
using CEC.Web.SRV.Models.Grid;

namespace CEC.Web.SRV.Profiles
{
	public class ExporterProfile : Profile
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
                .Include<SaiseExporterStage, SaiseExporterStageGridModel>();

		}
	}
}