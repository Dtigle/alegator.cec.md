using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;
using NHibernate;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace CEC.SRV.BLL
{
    public interface ISRVUserBll : IRepository
    {
        PageResponse<SRVIdentityUser> ListUsers(PageRequest request);

        UserProfileDto GetUserProfile(string userId);

        void StoreUserProfile(UserProfileDto dto);

        void UpdateAccount(string userId, string roleId, bool block, string comments);

        void DeleteAccount(string userId);

        void ChangeStatus(IEnumerable<string> accounts, bool block, string comments);
        void SetUserRegions(string userId, IEnumerable<long> regionIds);
		SRVIdentityUser GetRegistratorUser(string userId);
        void SetAccountLogoutTime();
    }

    public class SRVUserBll : Repository, ISRVUserBll
    {
        public SRVUserBll(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public void DeleteAccount(string userId)
        {
            var user = Query<SRVIdentityUser>().SingleOrDefault(x => x.Id == userId);
            if (user == null) throw new ArgumentNullException("user");
            if (user.IsBuiltIn) throw new SrvException("Account_IsBuiltInUsersNotDelete", "BuiltIn users cannot be deleted.");
            Delete(user);
        }

        public void ChangeStatus(IEnumerable<string> accounts, bool block, string comments)
        {
            var users = Query<SRVIdentityUser>().Where(x => accounts.Contains(x.Id) && !x.IsBuiltIn);
			var userLoggedId = SecurityHelper.GetLoggedUserId();

            foreach (var user in users)
            {
                if (block)
                {
	                if (user.Id != userLoggedId){
						user.Block();
	                }
                }
                else
                {
                    user.UnBlock();
                }
                user.Comments = comments;

                SaveOrUpdate(user);
            }
        }

        public void SetUserRegions(string userId, IEnumerable<long> regionIds)
        {
            var user = Get<SRVIdentityUser, string>(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.IsBuiltIn)
            {
                throw new Exception("User is Built In. Can't assign Regions");
            }

            if (user.Roles.Any(x => x.Name == "Administrator"))
            {
                throw new Exception("Administrators do not need Region restriction");
            }

            var currentRegionsIds = user.Regions.Select(x => x.Id).ToList();
            var removedRegions = currentRegionsIds.Except(regionIds);
            var addedRegions = regionIds.Except(currentRegionsIds);

            foreach (var removedRegionId in removedRegions)
            {
                var region = user.Regions.First(x => x.Id == removedRegionId);
                user.RemoveRegion(region);
            }

            foreach (var addedRegionId in addedRegions)
            {
                var region = LoadProxy<Region>(addedRegionId);
                user.AddRegion(region);
            }

            SaveOrUpdate(user);
        }

        public void UpdateAccount(string userId, string roleId, bool block, string comments)
        {
            var user = Query<SRVIdentityUser>().Fetch(x => x.Roles).SingleOrDefault(x => x.Id == userId);
			var userLoggedId = SecurityHelper.GetLoggedUserId();
            if (user == null) throw new ArgumentNullException("user");

            if (user.IsBuiltIn)
            {
				throw new SecurityException(MUI.Account_IsBuiltInRolesNotChange);
            }

			if(!string.IsNullOrEmpty(roleId))
			{
				var role = Get<IdentityRole, string>(roleId);
				if (role.Name == Transactions.Administrator)
				{
					foreach (var removedRegion in user.Regions.ToList())
					{
						user.RemoveRegion(removedRegion);
					}
				}

				if (user.Roles.All(x => x.Id != roleId))
				{
					if (userId == userLoggedId)
					{
						throw new SrvException("Account_", MUI.Account_ErrorNotChangeUsersStatus);
					}
					user.Roles.Clear();
					user.Roles.Add(role);
				}
			}


            if (block)
            {
				if (userId == userLoggedId){
					throw new SrvException("Account_", MUI.Account_ErrorNotChangeUsersStatus);
				}
                user.Block();
            }
            else
            {
                user.UnBlock();
            }
            user.Comments = comments;

            SaveOrUpdate(user);
        }

        public void StoreUserProfile(UserProfileDto dto)
        {
            var user = Session.QueryOver<SRVIdentityUser>().Where(x => x.Id == dto.Id).SingleOrDefault();
            var additionalInfo = user.AdditionalInfo ?? new AdditionalUserInfo();
            additionalInfo.FirstName = dto.FirstName;
            additionalInfo.LastName = dto.LastName;
            additionalInfo.DateOfBirth = dto.DateOfBirth;
            additionalInfo.Email = dto.Email;
            additionalInfo.LandlinePhone = dto.LandlinePhone;
            additionalInfo.MobilePhone = dto.MobilePhone;
            additionalInfo.WorkInfo = dto.WorkInfo;
	        additionalInfo.Gender = dto.Gender;
            user.AdditionalInfo = additionalInfo;

            SaveOrUpdate(user);
        }

        public PageResponse<SRVIdentityUser> ListUsers(PageRequest request)
        {
            IdentityRole role = null;
            AdditionalUserInfo ai = null;
            //Region region = null;

            return Session.QueryOver<SRVIdentityUser>()
                .JoinAlias(u => u.Roles, () => role, JoinType.LeftOuterJoin)
                .JoinAlias(u => u.AdditionalInfo, () => ai, JoinType.LeftOuterJoin)
                //.JoinAlias(u => u.Regions, () => region, JoinType.LeftOuterJoin)
                //.TransformUsing(Transformers.DistinctRootEntity)
                .RootCriteria
                .CreatePage<SRVIdentityUser>(request);
        }

        public UserProfileDto GetUserProfile(string userId)
        {
            var user = Session.Query<SRVIdentityUser>().Fetch(x => x.AdditionalInfo).SingleOrDefault(x => x.Id == userId);
            if (user == null) throw new ArgumentNullException("user");
            var profile = new UserProfileDto
            {
                Id = user.Id,
                LoginName = user.UserName,
                LastLogin = user.LastLogin,
                LoginCreationDate = user.Created,
            };

            if (user.AdditionalInfo != null)
            {
                profile.FirstName = user.AdditionalInfo.FirstName;
                profile.LastName = user.AdditionalInfo.LastName;
                profile.DateOfBirth = user.AdditionalInfo.DateOfBirth;
                profile.Email = user.AdditionalInfo.Email;
                profile.LandlinePhone = user.AdditionalInfo.LandlinePhone;
                profile.MobilePhone = user.AdditionalInfo.MobilePhone;
                profile.WorkInfo = user.AdditionalInfo.WorkInfo;
	            profile.Gender = user.AdditionalInfo.Gender;
            }

            return profile;
        }
		public SRVIdentityUser GetRegistratorUser(string userId)
		{
			var user = Get<SRVIdentityUser, string>(userId);
			if (user.IsBuiltIn || user.Roles.Any(x => x.Name == Transactions.Administrator))
			{
				throw new SrvException("Account_UserNotSetRegions", MUI.Account_UserNotSetRegions);
			}

			return user;
		}

        public void SetAccountLogoutTime()
        {
            var user = Get<SRVIdentityUser, string>(SecurityHelper.GetLoggedUserId());
            user.LastLogout = DateTimeOffset.UtcNow;
            SaveOrUpdate(user);
        }
    }
}
