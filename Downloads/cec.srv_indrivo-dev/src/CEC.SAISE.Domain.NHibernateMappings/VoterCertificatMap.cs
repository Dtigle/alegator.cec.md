using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
   public class VoterCertificatMap : IAutoMappingOverride<VoterCertificat>
    {
        public void Override(AutoMapping<VoterCertificat> mapping)
        {
            mapping.Table("VoterCertificat");
            mapping.Id(x => x.Id).Column("VoterCertificatId");
            mapping.References(x => x.AssignedVoter).Not.Nullable();
            mapping.Map(x => x.ReleaseDate).Nullable().Column("ReleaseDate");
            mapping.Map(x => x.CertificatNr).Length(255).Not.Nullable().Column("CertificatNr");
            mapping.References(x => x.PollingStation).Not.Nullable().Column("PollingStationId");
            mapping.Map(x => x.DeletedDate).Nullable().Column("Deleted");
           


        }
    }
}
