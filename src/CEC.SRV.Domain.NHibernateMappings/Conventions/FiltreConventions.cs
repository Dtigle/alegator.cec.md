using Amdaris.Domain;
using Amdaris.Domain.Identity;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace CEC.SRV.Domain.NHibernateMappings.Conventions
{
    public class FiltreConventions : IClassConvention, IClassConventionAcceptance
    {
        public void Apply(IClassInstance instance)
        {
            instance.ApplyFilter<DeleteOwnFilter>();
        }

        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.Expect(x => typeof (ISoftDeletable<IdentityUser>).IsAssignableFrom(x.EntityType));
        }
    }
}