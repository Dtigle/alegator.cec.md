using System;
using Amdaris.NHibernateProvider.Test;
using CEC.SRV.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Envers;
using NHibernate.Envers.Query;

namespace CEC.SRV.Database.Tests
{
    [TestClass]
    public class SRVIdentityUserTests : BaseRepositoryTests
    {
        protected override void LoadData()
        {

        }

        [TestMethod]
        public void CanCreateSRVIdentityUser()
        {
            var user = new SRVIdentityUser { UserName = "SRVIdentityUserTests", PasswordHash = "###hash###", IsBuiltIn = true};
            user.Block();

            Session.SaveOrUpdate(user);
            Session.Flush();

            Session.Evict(user);

            var dbUser = Session.Get<SRVIdentityUser>(user.Id);

            Assert.IsNotNull(dbUser);
            Assert.AreEqual(user.UserName, dbUser.UserName);
            Assert.AreEqual(user.IsBuiltIn, dbUser.IsBuiltIn);
            Assert.AreEqual(user.IsBlocked, dbUser.IsBlocked);
            Assert.AreEqual(user.BlockedDate, dbUser.BlockedDate);
            Assert.IsTrue(dbUser.BlockedDate.HasValue);
        }

        [TestMethod]
        public void UserIsAudited()
        {
            var userName = "SRVIdentityUserTests" + Guid.NewGuid().ToString("N");
            var user = CreateAndUpdateUser(userName);
            var userRev1 = Session.Auditer()
                .CreateQuery()
                .ForRevisionsOf<SRVIdentityUser>()
                .Add(AuditEntity.Id().Eq(user.Id))
                .SetMaxResults(1)
                .Single();

            Assert.IsNotNull(userRev1);
            Assert.AreEqual(userName, userRev1.UserName);
        }

        private SRVIdentityUser CreateAndUpdateUser(string userName)
        {
            SRVIdentityUser user;
            using (var tx = Session.BeginTransaction())
            {
                user = new SRVIdentityUser { UserName = userName, Created = DateTimeOffset.Now };
                user.Block();
                Session.SaveOrUpdate(user);

                tx.Commit();
            }

            using (var tx = Session.BeginTransaction())
            {
                user.UserName = userName + "Rename";
                Session.SaveOrUpdate(user);

                tx.Commit();
            }
            return user;
        }

    }
}
