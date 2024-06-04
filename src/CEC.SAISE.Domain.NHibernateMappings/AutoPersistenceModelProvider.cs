using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.NHibernateProvider.Mapping;
using FluentNHibernate.Automapping;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class AutoPersistenceModelProvider : IAutoPersistenceModelProvider
    {
        public void AlterPersistenceModel(AutoPersistenceModel model)
        {
            //model.IgnoreBase(typeof(SaiseBaseEntity));

            model.AddEntityAssembly(typeof(SaiseBaseEntity).Assembly);
            model.AddMappingsFromAssemblyOf<AutoPersistenceModelProvider>();
            model.UseOverridesFromAssemblyOf<AutoPersistenceModelProvider>();
        }
    }
}
