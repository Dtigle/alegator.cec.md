using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    [Obsolete]
    public class PersonByConflictMap : IAutoMappingOverride<PersonByConflict>
    {
        public void Override(AutoMapping<PersonByConflict> mapping)
        {
            mapping.Id(x => x.Id, "rspModificationDataId");
            mapping.Table("[v_PersonByConflict]");
            mapping.Schema(Schemas.Importer);
            mapping.ReadOnly();
        }
    }
}