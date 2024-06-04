using CEC.SRV.Domain.Importer;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PersonRawMap : IAutoMappingOverride<PersonRaw>
    {
        public void Override(AutoMapping<PersonRaw> mapping)
        {

            
        }
    }


    public class AlegatorDataMap : IAutoMappingOverride<AlegatorData>
    {
        public void Override(AutoMapping<AlegatorData> mapping)
        {
            
        }
    }

    public class RspDataMap : IAutoMappingOverride<RspData>
    {
        public void Override(AutoMapping<RspData> mapping)
        {
            
        }
    }
}