using System;
using CEC.SRV.Domain.Print;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PrintSessionMap : IAutoMappingOverride<PrintSession>
    {
        public void Override(AutoMapping<PrintSession> mapping)
        {
            mapping.References(x => x.Election).Not.Nullable();
            mapping.Map(x => x.Status).CustomType<EnumType<PrintStatus>>().Not.Nullable();
            mapping.HasMany(x => x.ExportPollingStations).Access.CamelCaseField(Prefix.Underscore).Cascade.All();
        }
    }
}