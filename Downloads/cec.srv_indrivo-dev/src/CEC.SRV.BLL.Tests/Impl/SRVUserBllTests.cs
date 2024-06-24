using System;
using System.Linq;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using Moq;
using NHibernate.Envers;
using Amdaris.NHibernateProvider;
using NHibernate.Envers.Query;
using NHibernate.Envers.Query.Criteria;
using CEC.SRV.BLL.Exceptions;
using NHibernate.Linq;
using NHibernate.Mapping;
using System.Collections.Generic;
using System.Security;
using CEC.Web.SRV.Resources;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class SRVUserBllTests : BaseTests<SRVUserBll, SRVUserBll>
    {
        [TestInitialize]
        public new void Startup()
        {
            IsMockedRepository = false;
        }

        public SRVUserBllTests()
        {
            Bll = new SRVUserBll(SessionFactory);
            Repository = Bll;
        }

        #region DeleteAccount

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: user")]
        public void DeleteAccountForNonExistingUserId_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var userId = GetNonExistingUserId();
            
            // Act

            Bll.DeleteAccount(userId);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(SrvException), "BuiltIn users cannot be deleted.")]
        public void DeleteAccountForBuiltInUser_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var user = GetFirstObjectFromDbTable(x => x.IsBuiltIn, GetSrvIdentityUser);
            var userId = user.Id;

            // Act

            Bll.DeleteAccount(userId);
        }

        [TestMethod]
        public void DeleteAccountForNonBuiltInUser_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var user = GetFirstObjectFromDbTable(x => !x.IsBuiltIn, GetNonBuiltInSrvIdentityUser);
            var userId = user.Id;

            // Act

            Bll.DeleteAccount(userId);

            // Assert

            var checkUser = GetFirstObjectFromDbTable<SRVIdentityUser>(x => x.Id == userId);
            Assert.IsNull(checkUser);
        }

        #endregion DeleteAccount

        #region ChangeStatus

        [TestMethod]
        public void ChangeStatusInBlocked_has_correct_logic()
        {
            ChangeStatusTest(true);
        }

        [TestMethod]
        public void ChangeStatusInUnBlocked_has_correct_logic()
        {
            ChangeStatusTest(false);
        }

        private void ChangeStatusTest(bool block)
        {
            // Arrange

            SetAdministratorRole();

            var loggedUser = GetCurrentUser();
            var loggedUserId = loggedUser.Id;
            var loggedUserIsBlocked = loggedUser.IsBlocked;
            var loggedUserBlockedDate = loggedUser.BlockedDate;
            var loggedUserComments = loggedUser.Comments;
            var loggedUserLoginAttempts = loggedUser.LoginAttempts;

            var builtInUser = GetFirstObjectFromDbTable(x => x.IsBuiltIn, GetSrvIdentityUser);
            var builtInUserId = builtInUser.Id;
            var builtInUserIsBlocked = builtInUser.IsBlocked;
            var builtInUserBlockedDate = builtInUser.BlockedDate;
            var builtInUserComments = builtInUser.Comments;
            var builtInUserLoginAttempts = builtInUser.LoginAttempts;

            var nonBuiltInUser = GetFirstObjectFromDbTable(x => !x.IsBuiltIn, GetNonBuiltInSrvIdentityUser);
            var nonBuiltInUserId = nonBuiltInUser.Id;

            var nonChangedUser = GetFirstObjectFromDbTable(x => (!x.IsBuiltIn) && (x.Id != nonBuiltInUserId), () =>
            {
                var ncUser = GetNonBuiltInSrvIdentityUser();
                ncUser.UserName = "nc";
                return ncUser;
            });
            var nonChangedUserId = nonChangedUser.Id;
            var nonChangedUserIsBlocked = nonChangedUser.IsBlocked;
            var nonChangedUserBlockedDate = nonChangedUser.BlockedDate;
            var nonChangedUserComments = nonChangedUser.Comments;
            var nonChangedUserLoginAttempts = nonChangedUser.LoginAttempts;

            var nonExistingUserId = GetNonExistingUserId();

            var accounts = new List<string>()
            {
                loggedUserId,
                builtInUserId,
                nonBuiltInUserId,
                nonExistingUserId
            };

            const string comments = "generated";

            var startDateAndTime = DateTime.Now;

            // Act

            Bll.ChangeStatus(accounts, block, comments);

            // Assert

            var endDateAndTime = DateTime.Now;

            var newLoggedUser = GetFirstObjectFromDbTable<SRVIdentityUser>(x => x.Id == loggedUserId);
            Assert.IsNotNull(newLoggedUser);
            Assert.AreEqual((block || loggedUser.IsBuiltIn) && loggedUserIsBlocked, newLoggedUser.IsBlocked);
            Assert.AreEqual(block ? loggedUserBlockedDate : (loggedUser.IsBuiltIn ? loggedUserBlockedDate : null), newLoggedUser.BlockedDate);
            Assert.AreEqual(loggedUser.IsBuiltIn ? loggedUserComments : comments, newLoggedUser.Comments);
            Assert.AreEqual(block ? loggedUserLoginAttempts : (loggedUser.IsBuiltIn ? loggedUserLoginAttempts : 0), newLoggedUser.LoginAttempts);

            var newBuiltInUser = GetFirstObjectFromDbTable<SRVIdentityUser>(x => x.Id == builtInUserId);
            Assert.IsNotNull(newBuiltInUser);
            Assert.AreEqual(builtInUserIsBlocked, newBuiltInUser.IsBlocked);
            Assert.AreEqual(builtInUserBlockedDate, newBuiltInUser.BlockedDate);
            Assert.AreEqual(builtInUserComments, newBuiltInUser.Comments);
            Assert.AreEqual(builtInUserLoginAttempts, newBuiltInUser.LoginAttempts);

            var newNonChangedUser = GetFirstObjectFromDbTable<SRVIdentityUser>(x => x.Id == nonChangedUserId);
            Assert.IsNotNull(newNonChangedUser);
            Assert.AreEqual(nonChangedUserIsBlocked, newNonChangedUser.IsBlocked);
            Assert.AreEqual(nonChangedUserBlockedDate, newNonChangedUser.BlockedDate);
            Assert.AreEqual(nonChangedUserComments, newNonChangedUser.Comments);
            Assert.AreEqual(nonChangedUserLoginAttempts, newNonChangedUser.LoginAttempts);

            var newNonBuiltInUser = GetFirstObjectFromDbTable<SRVIdentityUser>(x => x.Id == nonBuiltInUserId);
            Assert.IsNotNull(newNonBuiltInUser);
            Assert.AreEqual(block, newNonBuiltInUser.IsBlocked);
            if (block)
            {
                Assert.IsNotNull(newNonBuiltInUser.BlockedDate);
                Assert.IsTrue(newNonBuiltInUser.BlockedDate >= startDateAndTime);
                Assert.IsTrue(newNonBuiltInUser.BlockedDate <= endDateAndTime);
            }
            else
            {
                Assert.IsNull(newNonBuiltInUser.BlockedDate);
            }
            Assert.AreEqual(comments, newNonBuiltInUser.Comments);
            Assert.AreEqual(0, newNonBuiltInUser.LoginAttempts);

            var newNonExistingUser = GetFirstObjectFromDbTable<SRVIdentityUser>(x => x.Id == nonExistingUserId);
            Assert.IsNull(newNonExistingUser);
        }

        #endregion ChangeStatus

        #region SetUserRegions

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(Exception), "User not found")]
        public void SetUserRegionsForNullUser_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var userId = GetNonExistingUserId();
            
            // Act

            Bll.SetUserRegions(userId, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(Exception), "User is Built In. Can't assign Regions")]
        public void SetUserRegionsForBuiltInUser_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var user = GetFirstObjectFromDbTable(x => x.IsBuiltIn, GetSrvIdentityUser);
            var userId = user.Id;

            // Act

            Bll.SetUserRegions(userId, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(Exception), "Administrators do not need Region restriction")]
        public void SetUserRegionsForAdminUser_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var user = GetFirstObjectFromDbTable(x => (!x.IsBuiltIn) && x.Roles.Any(y => y.Name == "Administrator"), GetNonBuiltInSrvIdentityUser);
            var userId = user.Id;

            // Act

            Bll.SetUserRegions(userId, null);
        }

        [TestMethod]
        public void SetUserRegionsForRegistratorUser_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region1 = GetFirstObjectFromDbTable(GetRegion);
            var region2 = GetFirstObjectFromDbTable(x => x.Id != region1.Id, GetRegionWithRegistruId);
            var region3 = GetFirstObjectFromDbTable(x => (x.Id != region1.Id) && (x.Id != region2.Id), GetRegionWithRank1);

            var user = GetFirstObjectFromDbTable(x => (!x.IsBuiltIn) && x.Regions.Any(), () =>
            {
                var srvUser = GetNonBuiltInSrvIdentityUser();
                srvUser.Roles.Where(x => x.Name == "Administrator").ForEach(x => x.Name = "Registrator");
                srvUser.AddRegion(region1);
                srvUser.AddRegion(region3);
                return srvUser;
            });
            var userId = user.Id;
            
            var regionIds = new List<long>() { region1.Id, region2.Id};

            // Act

            Bll.SetUserRegions(userId, regionIds);

            // Assert

            var newUser = GetFirstObjectFromDbTable<SRVIdentityUser>(x => x.Id == userId);

            AssertObjectListsAreEqual(regionIds, newUser.Regions.Select(x => x.Id).ToList());
        }

        #endregion SetUserRegions

        #region UpdateAccount

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: user")]
        public void UpdateAccountForNonExistingUserId_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var userId = GetNonExistingUserId();

            // Act

            Bll.UpdateAccount(userId, null, false, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(SecurityException), "Rolul și statutul utilizatorilor de sistem (System si Administrator) nu poate fi modificat")]
        public void UpdateAccountForBuiltInUser_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var user = GetFirstObjectFromDbTable(x => x.IsBuiltIn, GetSrvIdentityUser);
            var userId = user.Id;

            // Act

            Bll.UpdateAccount(userId, null, false, null);
        }
        
        [TestMethod]
        public void UpdateAccountForNonBuiltInLoggedUserAndAnotherAdminRole_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            AddRegionToCurrentUser(region);

            var currentUser = GetCurrentUser();
            var userComments = currentUser.Comments;
            var userId = currentUser.Id;
            currentUser.IsBuiltIn = false;
            Bll.SaveOrUpdate(currentUser);

            var role = GetFirstObjectFromDbTable(GetIdentityRole, true);
            var roleId = role.Id;

            const string comments = "comentarii...";

            // Act

            SafeExec(Bll.UpdateAccount, userId, roleId, false, comments, true, false, "Account_", MUI.Account_ErrorNotChangeUsersStatus);
            
            // Assert

            var newCurrentUser = GetCurrentUser();
            Assert.IsNotNull(newCurrentUser);
            CheckIfUserHasNotRegions(newCurrentUser);
            Assert.AreEqual(userComments, newCurrentUser.Comments);
        }
        
        [TestMethod]
        public void UpdateAccountForNonBuiltInLoggedUserAndAnotherRegistratorRole_throws_an_exception()
        {
            // Arrange

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            AddRegionToCurrentUser(region);

            var currentUser = GetCurrentUser();
            var userComments = currentUser.Comments;
            var userId = currentUser.Id;
            currentUser.IsBuiltIn = false;
            Bll.SaveOrUpdate(currentUser);

            var role = GetFirstObjectFromDbTable(GetRegistratorIdentityRole, true);
            var roleId = role.Id;

            const string comments = "comentarii...";

            // Act

            SafeExec(Bll.UpdateAccount, userId, roleId, false, comments, true, false, "Account_", MUI.Account_ErrorNotChangeUsersStatus);

            // Assert

            var newCurrentUser = GetCurrentUser();
            Assert.IsNotNull(newCurrentUser);
            CheckIfUserContainsTheRegion(newCurrentUser, region);
            Assert.AreEqual(userComments, newCurrentUser.Comments);
        }

        [TestMethod]
        public void UpdateAccountToBlockStateForNonBuiltInLoggedUserAndExistingAdminRole_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            AddRegionToCurrentUser(region);

            var currentUser = GetCurrentUser();
            var userComments = currentUser.Comments;
            var userId = currentUser.Id;
            currentUser.IsBuiltIn = false;
            var rol = GetFirstObjectFromDbTable(x => x.Name == "Registrator", GetRegistratorIdentityRole);
            currentUser.Roles.Add(rol);
            Bll.SaveOrUpdate(currentUser);

            var role = currentUser.Roles.First(x => x.Name == "Administrator");
            var roleId = role.Id;

            const string comments = "comentarii...";

            // Act

            SafeExec(Bll.UpdateAccount, userId, roleId, true, comments, true, false, "Account_", MUI.Account_ErrorNotChangeUsersStatus);

            // Assert

            var newCurrentUser = GetCurrentUser();
            Assert.IsNotNull(newCurrentUser);
            CheckIfUserHasNotRegions(newCurrentUser);
            CheckIfUserContainsTheRole(newCurrentUser, role);
            CheckIfUserContainsTheRole(newCurrentUser, rol);
            Assert.AreEqual(userComments, newCurrentUser.Comments);
        }

        [TestMethod]
        public void UpdateAccountToBlockStateForNonBuiltInLoggedUserAndExistingRegistratorRole_throws_an_exception()
        {
            // Arrange

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            AddRegionToCurrentUser(region);

            var currentUser = GetCurrentUser();
            var userComments = currentUser.Comments;
            var userId = currentUser.Id;
            currentUser.IsBuiltIn = false;

            var role = GetFirstObjectFromDbTable(x => x.Name == "Registrator", GetRegistratorIdentityRole);
            var roleId = role.Id;
            
            currentUser.Roles.Add(role);
            Bll.SaveOrUpdate(currentUser);

            const string comments = "comentarii...";

            // Act

            SafeExec(Bll.UpdateAccount, userId, roleId, true, comments, true, false, "Account_", MUI.Account_ErrorNotChangeUsersStatus);

            // Assert

            var newCurrentUser = GetCurrentUser();
            Assert.IsNotNull(newCurrentUser);
            CheckIfUserContainsTheRegion(newCurrentUser, region);
            CheckIfUserContainsTheRole(newCurrentUser, role);
            Assert.AreEqual(2, newCurrentUser.Roles.Count);
            Assert.AreEqual(userComments, newCurrentUser.Comments);
        }
        
        [TestMethod]
        public void UpdateAccountToBlockStateForNonBuiltInNonLoggedUserAndAnotherRegistratorRole_has_correct_logic()
        {   
            UpdateAccountForNonBuiltInNonLoggedUserTest(true, false, true);
        }

        [TestMethod]
        public void UpdateAccountToUnBlockStateForNonBuiltInNonLoggedUserAndAnotherRegistratorRole_has_correct_logic()
        {
            UpdateAccountForNonBuiltInNonLoggedUserTest(false, false, true);
        }

        [TestMethod]
        public void UpdateAccountToBlockStateForNonBuiltInNonLoggedUserAndAnotherAdminRole_has_correct_logic()
        {
            UpdateAccountForNonBuiltInNonLoggedUserTest(true, true, true);
        }

        [TestMethod]
        public void UpdateAccountToUnBlockStateForNonBuiltInNonLoggedUserAndAnotherAdminRole_has_correct_logic()
        {
            UpdateAccountForNonBuiltInNonLoggedUserTest(false, true, true);
        }

        [TestMethod]
        public void UpdateAccountToBlockStateForNonBuiltInNonLoggedUserAndExistingRegistratorRole_has_correct_logic()
        {
            UpdateAccountForNonBuiltInNonLoggedUserTest(true, false, false);
        }

        [TestMethod]
        public void UpdateAccountToUnBlockStateForNonBuiltInNonLoggedUserAndExistingRegistratorRole_has_correct_logic()
        {
            UpdateAccountForNonBuiltInNonLoggedUserTest(false, false, false);
        }

        [TestMethod]
        public void UpdateAccountToBlockStateForNonBuiltInNonLoggedUserAndExistingAdminRole_has_correct_logic()
        {
            UpdateAccountForNonBuiltInNonLoggedUserTest(true, true, false);
        }

        [TestMethod]
        public void UpdateAccountToUnBlockStateForNonBuiltInNonLoggedUserAndExistingAdminRole_has_correct_logic()
        {
            UpdateAccountForNonBuiltInNonLoggedUserTest(false, true, false);
        }

        private void UpdateAccountForNonBuiltInNonLoggedUserTest(bool block, bool isAdmin, bool isAnotherRole)
        {
            // Arrange

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);

            var currentUserId = SecurityHelper.GetLoggedUserId();
            var user = GetFirstObjectFromDbTable(x => (x.Id != currentUserId) && (!x.IsBuiltIn) && (x.Regions.Count == 1) && (x.Roles.Count > 1),
                () =>
                {
                    var srvUser = GetNonBuiltInSrvIdentityUser();
                    srvUser.AddRegion(region);
                    var rol = GetFirstObjectFromDbTable(x => x.Name == "Registrator", GetRegistratorIdentityRole);
                    srvUser.Roles.Add(rol);
                    return srvUser;
                });
            var userId = user.Id;

            var role = isAnotherRole
                ? (isAdmin
                    ? GetFirstObjectFromDbTable(GetIdentityRole, true)
                    : GetFirstObjectFromDbTable(GetRegistratorIdentityRole, true))
                : user.Roles.First(x => x.Name == (isAdmin ? "Administrator" : "Registrator"));
            var roleId = role.Id;

            const string comments = "utilizator cu statut schimbat";

            var startDateTime = DateTime.Now;

            // Act

            SafeExec(Bll.UpdateAccount, userId, roleId, block, comments);

            // Assert

            var endDateTime = DateTime.Now;

            var newUser = GetFirstObjectFromDbTable<SRVIdentityUser>(x => x.Id == userId);
            Assert.IsNotNull(newUser);

            if (isAdmin) CheckIfUserHasNotRegions(newUser); else CheckIfUserContainsTheRegion(newUser, region);
            if (isAnotherRole) CheckIffUserContainsTheRole(newUser, role); else CheckIfUserContainsTheRole(newUser, role);
            if (block) CheckIfUserIsBlocked(newUser, startDateTime, endDateTime); else CheckIfUserIsUnBlocked(newUser);

            Assert.AreEqual(comments, newUser.Comments);
        }

        private void CheckIfUserHasNotRegions(SRVIdentityUser user)
        {
            Assert.IsNotNull(user.Regions);
            Assert.AreEqual(0, user.Regions.Count);
        }

        private void CheckIfUserContainsTheRegion(SRVIdentityUser user, Region region)
        {
            Assert.IsNotNull(user.Regions);
            Assert.IsTrue(user.Regions.Contains(region));
        }

        private void CheckIffUserContainsTheRole(SRVIdentityUser user, IdentityRole role)
        {
            CheckIfUserContainsTheRole(user, role);
            Assert.AreEqual(1, user.Roles.Count);
        }

        private void CheckIfUserContainsTheRole(SRVIdentityUser user, IdentityRole role)
        {
            Assert.IsNotNull(user.Roles);
            Assert.IsTrue(user.Roles.Contains(role));
        }

        private void CheckIfUserIsBlocked(SRVIdentityUser user, DateTime start, DateTime end)
        {
            Assert.IsTrue(user.IsBlocked);
            Assert.AreEqual(0, user.LoginAttempts);
            Assert.IsNotNull(user.BlockedDate);
            Assert.IsTrue(user.BlockedDate >= start);
            Assert.IsTrue(user.BlockedDate <= end);
        }

        private void CheckIfUserIsUnBlocked(SRVIdentityUser user)
        {
            Assert.IsFalse(user.IsBlocked);
            Assert.AreEqual(0, user.LoginAttempts);
            Assert.IsNull(user.BlockedDate);
        }

        #endregion UpdateAccount

        [TestMethod]
        public void StoreUserProfile_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var dto = new UserProfileDto()
            {
                FirstName = "firstName",
                LastName = "lastName",
                DateOfBirth = DateTime.Now,
                Email = "email",
                LandlinePhone = "landlinePhone",
                MobilePhone = "mobilePhone",
                WorkInfo = "workInfo",
                Gender = GetFirstObjectFromDbTable(GetGender),
                Id = SecurityHelper.GetLoggedUserId()
            };

            // Act

            SafeExec(Bll.StoreUserProfile, dto);

            // Assert

            var user = GetCurrentUser();

            Assert.IsNotNull(user);
            Assert.AreEqual(dto.Id, user.Id);
            Assert.IsNotNull(user.AdditionalInfo);
            Assert.AreEqual(dto.FirstName, user.AdditionalInfo.FirstName);
            Assert.AreEqual(dto.LastName, user.AdditionalInfo.LastName);
            Assert.AreEqual(dto.DateOfBirth, user.AdditionalInfo.DateOfBirth);
            Assert.AreEqual(dto.Email, user.AdditionalInfo.Email);
            Assert.AreEqual(dto.LandlinePhone, user.AdditionalInfo.LandlinePhone);
            Assert.AreEqual(dto.MobilePhone, user.AdditionalInfo.MobilePhone);
            Assert.AreEqual(dto.WorkInfo, user.AdditionalInfo.WorkInfo);
            Assert.AreSame(dto.Gender, user.AdditionalInfo.Gender);
        }

        [TestMethod]
        public void ListUsers_returns_correct_result()
        {
            // Arrange

            var expUsers = GetAllObjectsFromDbTable<SRVIdentityUser>();

            var pageRequest = GetPageRequest();
            pageRequest.PageSize = expUsers.Count + 10;

            // Act

            var usersPage = SafeExecFunc(Bll.ListUsers, pageRequest);
        
            // Assert

            Assert.IsNotNull(usersPage);
            Assert.IsNotNull(usersPage.Items);
            AssertObjectListsAreEqual(expUsers.Select(x => x.Id).ToList(), usersPage.Items.Select(x => x.Id).ToList());
        }

        #region GetUserProfile

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: user")]
        public void GetUserProfileForNonExistingUserId_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var userId = GetNonExistingUserId();

            // Act

            Bll.GetUserProfile(userId);
        }

        [TestMethod]
        public void GetUserProfileForExistingUserId_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var user = GetCurrentUser();
            var userId = user.Id;

            // Act

            var dto = SafeExecFunc(Bll.GetUserProfile, userId);

            // Assert

            Assert.IsNotNull(dto);
            Assert.AreEqual(user.UserName, dto.LoginName);
            Assert.AreEqual(user.LastLogin, dto.LastLogin);
            Assert.AreEqual(user.Created, dto.LoginCreationDate);

            if (user.AdditionalInfo != null)
            {
                Assert.AreEqual(user.AdditionalInfo.FirstName, dto.FirstName);
                Assert.AreEqual(user.AdditionalInfo.LastName, dto.LastName);
                Assert.AreEqual(user.AdditionalInfo.DateOfBirth, dto.DateOfBirth);
                Assert.AreEqual(user.AdditionalInfo.Email, dto.Email);
                Assert.AreEqual(user.AdditionalInfo.LandlinePhone, dto.LandlinePhone);
                Assert.AreEqual(user.AdditionalInfo.MobilePhone, dto.MobilePhone);
                Assert.AreEqual(user.AdditionalInfo.WorkInfo, dto.WorkInfo);
                Assert.AreSame(user.AdditionalInfo.Gender, dto.Gender);
            }
        }

        #endregion GetUserProfile

        #region GetRegistratorUser

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetRegistratorUserByNonExistingUserId_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var userId = GetNonExistingUserId();

            // Act

            Bll.GetRegistratorUser(userId);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(SrvException), "Administratorilor nu li se pot asocia localități")]
        public void GetRegistratorUserByBuiltInUserId_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var user = GetFirstObjectFromDbTable(x => x.IsBuiltIn, GetSrvIdentityUser);
            var userId = user.Id;

            // Act

            Bll.GetRegistratorUser(userId);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(SrvException), "Administratorilor nu li se pot asocia localități")]
        public void GetRegistratorUserByNonBuiltInAdminUserId_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var user = GetFirstObjectFromDbTable(x => (!x.IsBuiltIn) && x.Roles.Any(y => y.Name == RolesStrings.Administrator), GetNonBuiltInSrvIdentityUser);
            var userId = user.Id;

            // Act

            Bll.GetRegistratorUser(userId);
        }

        [TestMethod]
        public void GetRegistratorUserByNonBuiltInRegistratorUserId_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expUser = GetFirstObjectFromDbTable(x => (!x.IsBuiltIn) && x.Roles.All(y => y.Name != "Administrator"),
                () =>
                {
                    var srvUser = GetNonBuiltInSrvIdentityUser();
                    srvUser.Roles.Where(x => x.Name == "Administrator").ForEach(x => x.Name = "Registrator");
                    return srvUser;
                });
            var userId = expUser.Id;

            // Act

            var user = SafeExecFunc(Bll.GetRegistratorUser, userId);

            // Assert

            Assert.AreSame(expUser, user);
        }

        #endregion GetRegistratorUser

        [TestMethod]
        public void SetAccountLogoutTime_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var startDateTime = DateTimeOffset.UtcNow;

            // Act

            SafeExec(Bll.SetAccountLogoutTime);

            // Assert

            var endDateTime = DateTimeOffset.UtcNow;

            var user = GetCurrentUser();
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.LastLogout);
            Assert.IsTrue(user.LastLogout >= startDateTime);
            Assert.IsTrue(user.LastLogout <= endDateTime);
        }

        private string GetNonExistingUserId()
        {
            string userId;
            SRVIdentityUser user;

            do
            {
                userId = Guid.NewGuid().ToString();
                user = GetFirstObjectFromDbTable<SRVIdentityUser>(x => x.Id == userId);
            } while (user != null);

            return userId;
        }
    }
}
