using System;
using System.Linq;
using Amdaris.Domain;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Test;
using CEC.SRV.Domain.Lookup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CEC.SRV.Database.Tests
{
    [TestClass]
    public class StreetTests : BaseRepositoryTests
    {
        private IRepository _repository;

        private Region _region;

        private StreetType _streetType;

        public StreetTests()
        {
            _repository = new Repository(SessionFactory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CantCreateStreetWithNullRegion()
        {
            new Street(null, _streetType, "some name");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CantCreateStreetWithRegionWithHasStreetsFalse()
        {
            _region.HasStreets = false;

            new Street(_region, _streetType, "some name");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CantCreateStreetWithStreetTypeNull()
        {
            new Street(_region, null, "some name");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CantCreateStreetWithEmptyName()
        {
            new Street(_region, _streetType, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CantCreateStreetWithNullName()
        {
            new Street(_region, _streetType, null);
        }

        [TestMethod]
        public void CanCreateStreet()
        {
            var street = CreateStreet();

            Assert.IsFalse(street.IsTransient());
        }

        [TestMethod]
        public void CanGetStreet()
        {
            var street = CreateStreet();

            var dbStreet = _repository.Query<Street>().Single(x => x.Name == street.Name);

            Assert.IsNotNull(dbStreet);
            Assert.AreSame(street, dbStreet);

        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CantChangeRegionWithNullValue()
        {
            var street = CreateStreet();
            street.ChangeRegion(null);
        }

        [TestMethod]
        [ExpectedException(typeof (NotSupportedException))]
        public void CantChangeRegionWithHasStreetsFalse()
        {
            var street = CreateStreet();
            _region.HasStreets = false;
            street.ChangeRegion(_region);
        }

        [TestMethod]
        public void CanChangeRegion()
        {
            var street = CreateStreet();
            var newRegion = CreateRegion("other region", true);

            street.ChangeRegion(newRegion);
            _repository.SaveOrUpdate(street);

            var dbStreet = _repository.Query<Street>().Single(x => x.Id == street.Id);

            Assert.AreSame(dbStreet.Region, newRegion);
        }

        private Street CreateStreet()
        {
            var street = new Street(_region, _streetType, "some name");
            _repository.SaveOrUpdate(street);
            return street;
        }


        protected override void LoadData()
        {
            _region = CreateRegion("Sarateni", true);

            _streetType = _repository.Query<StreetType>().Single(x => x.Name == "str.");
            
        }

        private Region CreateRegion(string name, bool hasStreet)
        {
            var regionType = _repository.Query<RegionType>().Single(x => x.Name == "com.");

            var region = new Region(regionType) {Name = name, HasStreets = hasStreet};

            _repository.SaveOrUpdate(region);

            return region;
        }
    }
}