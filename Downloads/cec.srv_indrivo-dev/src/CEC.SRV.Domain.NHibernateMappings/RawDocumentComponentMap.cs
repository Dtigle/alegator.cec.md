using CEC.SRV.Domain.Importer;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class RawDocumentComponentMap : ComponentMap<RawDocument>
    {
        public RawDocumentComponentMap()
        {
            Map(x => x.ExternalType);
            Map(x => x.Serial);
            Map(x => x.Number);
        }
    }
}