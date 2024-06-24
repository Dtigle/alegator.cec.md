using CEC.SRV.Domain.Print;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace CEC.SRV.Domain.NHibernateMappings.Conventions
{
    public class PrintConvention : IClassConvention, IClassConventionAcceptance
    {
        public void Apply(IClassInstance instance)
        {
            instance.Schema(Schemas.Print);
        }

        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.Expect(x => typeof(PrintEntity).IsAssignableFrom(x.EntityType));
        }
    }
}