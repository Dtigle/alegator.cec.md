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
    public class RegionTests : BaseRepositoryTests
    {
        private RegionType _uta;
        private RegionType _raion;
        private readonly IRepository _repository;


        public RegionTests()
        {
            _repository = new Repository(SessionFactory);
        }


        protected override void LoadData()
        {
            _uta = GetRegionType("UTA");
            _raion = GetRegionType("r-n");

            Assert.IsNotNull(_uta);
            Assert.IsNotNull(_raion);
            Assert.IsTrue(_uta.Rank < _raion.Rank);


        }

        [TestMethod]
        public void CanAddRegion()
        {
            var region = CreateRegion(_uta);
            Assert.IsFalse(region.IsTransient());
        }

        [TestMethod]
        public void CanChangeParent()
        {
            var parent = CreateRegion(_uta);
            var secondParent = CreateRegion(_uta, null, "secondParent");
            var region = CreateRegion(_raion, parent);

            region.ChangeParent(secondParent);

            Assert.AreEqual(region.Parent, secondParent);
            Assert.IsTrue(secondParent.Children.Contains(region));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CantSetParentWithHigherRank()
        {
            var region = CreateRegion(_raion);
            CreateRegion(_uta, region);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CantSetParentAsSameInstance()
        {
            var parent = CreateRegion(_uta);
            parent.ChangeParent(parent);
        }

        [TestMethod]
        public void CanAddChild()
        {
            var uta = CreateRegion(_uta);
            var raion = CreateRegion(_raion);

            uta.AddChild(raion);

            Assert.IsTrue(uta.Children.Contains(raion));
            Assert.AreSame(raion.Parent, uta);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ThrowsExceptionIfChildHasLowerRank()
        {
            var uta = CreateRegion(_uta);
            var raion = CreateRegion(_raion);

            raion.AddChild(uta);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsExceptionIfAddedRegionIsNull()
        {
            var uta = CreateRegion(_uta);
            uta.AddChild(null);
        }

        [TestMethod]
        public void CanRemoveChild()
        {
            var uta = CreateRegion(_uta);
            var raion = CreateRegion(_raion, uta);

            Assert.IsTrue(uta.Children.Contains(raion));

            uta.RemoveChild(raion);

            Session.Flush();

            var dbUta = _repository.Query<Region>().Single(x => x.Id == uta.Id);
            Assert.IsFalse(dbUta.Children.Contains(raion));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsExceptionIfRemovedChildIsNull()
        {
            var uta = CreateRegion(_uta);
            uta.RemoveChild(null);
        }

        [TestMethod]
        public void CanGetRaionTree()
        {
            var uta = CreateRegion(_uta);
            var raion = CreateRegion(_raion, uta);
            Session.Flush();
            var dbUta = _repository.Query<Region>().Single(x => x.Id == uta.Id);

            Assert.IsNotNull(dbUta);

            Assert.IsTrue(dbUta.Children.Contains(raion));
        }

        [TestMethod]
        public void CanCalculateLevel()
        {
            var uta = CreateRegion(_uta);
            var raion = CreateRegion(_raion, uta);

            Assert.AreEqual(raion.Level, 1);
            Assert.AreEqual(uta.Level, 0);
        }

        private Region CreateRegion(RegionType regionType, Region parent = null, string name = "")
        {

            var regionName = string.IsNullOrWhiteSpace(name) ? regionType.Name : name;

            var region = parent == null
                ? new Region(regionType) { Name = regionName }
                : new Region(parent, regionType) { Name = regionName };

            _repository.SaveOrUpdate(region);

            return region;
        }

        private RegionType GetRegionType(string name)
        {
            return _repository.Query<RegionType>().Single(x => x.Name == name);
        }
    }
}