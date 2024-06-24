using System.Globalization;
using AutoMapper;
using CEC.SRV.Domain.Print;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Export;

namespace CEC.Web.SRV.Profiles
{
	public class PrintSessionProfile : Profile
	{
		protected override void Configure()
		{
			Mapper.CreateMap<PrintSession, PrintSessionsGridModel>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.Election.NameRo))
				.ForMember(x => x.StartDate, z => z.MapFrom(y => y.StartDate.HasValue ? y.StartDate.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
				.ForMember(x => x.EndDate, z => z.MapFrom(y => y.EndDate.HasValue ? y.EndDate.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
				.ForMember(x => x.CreatedById, z => z.MapFrom(y => y.CreatedBy.UserName))
				.ForMember(x => x.Status, z => z.MapFrom(y => y.Status.GetEnumDescription()))
				.ForMember(x => x.ModifiedById, z => z.MapFrom(y => y.ModifiedBy.UserName))
				.ForMember(x => x.DeletedById, z => z.MapFrom(y => y.DeletedBy.UserName))
				.ForMember(x => x.DataDeleted, z => z.MapFrom(y => y.Deleted.HasValue ? y.Deleted.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
				.ForMember(x => x.DataModified, z => z.MapFrom(y => y.Modified.HasValue ? y.Modified.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty))
				.ForMember(x => x.DataCreated, z => z.MapFrom(y => y.Created.HasValue ? y.Created.Value.LocalDateTime.ToString(CultureInfo.CurrentCulture) : string.Empty));
		}
	}
}