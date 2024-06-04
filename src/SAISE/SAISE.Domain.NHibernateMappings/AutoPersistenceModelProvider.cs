using Amdaris.NHibernateProvider.Mapping;
using FluentNHibernate.Automapping;

namespace SAISE.Domain.NHibernateMappings
{
    public class AutoPersistenceModelProvider : IAutoPersistenceModelProvider
    {
        public void AlterPersistenceModel(AutoPersistenceModel model)
        {
            model.IgnoreBase(typeof (SaiseEntity));

            model.AddEntityAssembly(typeof(SaiseEntity).Assembly);
            model.AddMappingsFromAssemblyOf<AutoPersistenceModelProvider>();
            model.UseOverridesFromAssemblyOf<AutoPersistenceModelProvider>();
        }
    }
}