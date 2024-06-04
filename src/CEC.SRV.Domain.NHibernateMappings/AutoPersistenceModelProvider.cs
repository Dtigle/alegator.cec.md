using Amdaris.NHibernateProvider.Mapping;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.NHibernateMappings.Conventions;
using FluentNHibernate.Automapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class AutoPersistenceModelProvider : IAutoPersistenceModelProvider
    {
        public void AlterPersistenceModel(AutoPersistenceModel model)
        {
            model.IgnoreBase(typeof(SRVBaseEntity));
            model.IgnoreBase((typeof (Lookup.Lookup)));
            model.IgnoreBase(typeof(RawData));
            model.IgnoreBase(typeof(PersonRaw));

            model.Conventions.Add<LookupConvention>();
            model.Conventions.Add<ImporterConvention>();
            model.Conventions.Add<ClassifierConvention>();
            model.Conventions.Add<SrvEntitiesConvention>();
            model.Conventions.Add<FiltreConventions>();
            model.Conventions.Add<PrintConvention>();

            model.AddEntityAssembly(typeof(SRVBaseEntity).Assembly);
            model.AddMappingsFromAssemblyOf<AutoPersistenceModelProvider>();
            model.UseOverridesFromAssemblyOf<AutoPersistenceModelProvider>();
        }
    }
}