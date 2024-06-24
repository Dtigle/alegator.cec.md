using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace CEC.SRV.Domain.NHibernateMappings.Conventions
{
    public class LookupConvention : IClassConvention, IClassConventionAcceptance
    {
        public void Apply(IClassInstance instance)
        {
            instance.Schema(Schemas.Lookup);
        }

        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.Expect(x => typeof(Lookup.Lookup).IsAssignableFrom(x.EntityType));
        }
    }
}