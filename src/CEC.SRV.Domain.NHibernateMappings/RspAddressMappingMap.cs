using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class RspAddressMappingMap : IAutoMappingOverride<RspAddressMapping>
    {
        public void Override(AutoMapping<RspAddressMapping> mapping)
        {
            mapping.ReadOnly();
            mapping.Table("v_RspAddressMappings");
            mapping.Schema(Schemas.Importer);

            mapping.Id(x => x.Id, "mappingAddressId");

            mapping.References(x => x.SrvRegion);
            mapping.Map(x => x.SrvAddressId);
            mapping.Map(x => x.SrvFullAddress);

            mapping.References(x => x.RspRegion);
            mapping.Map(x => x.RspRegionCode).Column("rspAdministrativeCode");
            mapping.Map(x => x.RspStreetCode);
            mapping.Map(x => x.RspStreetName);
            mapping.Map(x => x.RspHouseNumber);
            mapping.Map(x => x.RspHouseSuffix);
        }
    }
}
