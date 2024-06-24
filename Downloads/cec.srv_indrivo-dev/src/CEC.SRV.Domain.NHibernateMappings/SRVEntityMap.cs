using Amdaris.NHibernateProvider.Mapping;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public abstract class SRVEntityMap<T> : IAutoMappingOverride<T> where T : SRVBaseEntity
    {
        public virtual void Override(AutoMapping<T> mapping)
        {
            mapping.ApplyFilter<DeletedNullFilter>();
        }
    }
}
