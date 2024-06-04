using System.Globalization;
using AutoMapper;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Export;

namespace CEC.Web.SRV.Profiles
{
	public class SaiseExporterStageProfile : Profile
	{
		protected override void Configure()
		{
			Mapper.CreateMap<SaiseExporterStage, ExportRsaToSaiseGridModel>()
				.ForMember(x => x.Description, z => z.MapFrom(y => y.Description))
				.ForMember(x => x.Status, z => z.MapFrom(y => y.Status.GetEnumDescription()))
				.ForMember(x => x.ErrorMessage, z => z.MapFrom(y => y.ErrorMessage))
				.ForMember(x => x.CreatedById, z => z.MapFrom(y => y.CreatedBy.UserName))
				.ForMember(x => x.ModifiedById, z => z.MapFrom(y => y.ModifiedBy.UserName))
				.ForMember(x => x.DeletedById, z => z.MapFrom(y => y.DeletedBy.UserName))
				.ForMember(x => x.DataDeleted, z => z.MapFrom(y => y.Deleted.HasValue ? y.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
				.ForMember(x => x.DataModified, z => z.MapFrom(y => y.Modified.HasValue ? y.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
				.ForMember(x => x.DataCreated, z => z.MapFrom(y => y.Created.HasValue ? y.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
			    .ForMember(x => x.StartDate,z =>z.MapFrom(y => y.StartDate.HasValue ? y.StartDate.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
				.ForMember(x => x.EndDate,z =>z.MapFrom(y => y.EndDate.HasValue ? y.EndDate.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty));
		}
	}
}