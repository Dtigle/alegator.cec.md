using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class DeleteOwnFilter : FilterDefinition
    {
        public DeleteOwnFilter()
        {
            WithName("DeleteOwnFilter")
				.WithCondition("(deleted IS NULL OR (deletedById = :deletedBy and deleted > DATEADD(month, -1  , GETDATE())))")
                .AddParameter("deletedBy", NHibernate.NHibernateUtil.String);
        }
    }
}