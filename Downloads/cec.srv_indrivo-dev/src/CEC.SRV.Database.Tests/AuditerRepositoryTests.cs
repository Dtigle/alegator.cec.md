using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Test;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.Lookup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CEC.SRV.Database.Tests
{
    [TestClass]
    public class AuditerRepositoryTests : BaseRepositoryTests
    {
        private readonly IAuditerRepository _auditerRepository;
        private readonly IRepository _repository;

        public AuditerRepositoryTests()
        {
            _auditerRepository = new AuditerRepository(SessionFactory);
            _repository = new Repository(SessionFactory);
        }

        protected override void LoadData()
        {
            
        }

        [TestMethod]
        public void CanReadAuditForStreetType()
        {
            var streetType = new StreetType {Name = "TestStreetType"};
            _repository.SaveOrUpdate(streetType);
            var result = _auditerRepository.Get<StreetType>(new PageRequest(), streetType.Id);

            Assert.IsNotNull(result);
        }
    }
}
