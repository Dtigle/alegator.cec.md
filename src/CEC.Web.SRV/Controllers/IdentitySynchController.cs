using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Identity;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Models.Account;
using CEC.Web.SRV.Models.Synchronizer;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CEC.Web.SRV.Controllers
{
	public class IdentitySynchController : ApiController
	{
		public RoleManager<IdentityRole> RoleManager { get; private set; }
		private UserManager<SRVIdentityUser> UserManager { get; set; }

		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.Authorize]
		public string GetRsaUsers()
		{
			var sessionFactory = IoC.GetSessionFactory();
			var userBll = new SRVUserBll(sessionFactory);
			var userStore = new UserRepository<SRVIdentityUser>(sessionFactory);
			UserManager = new UserManager<SRVIdentityUser>(userStore);

			var list = new List<SynchUserViewModel>();
			var users = userBll.Query<SRVIdentityUser>().ToList();
			foreach (var user in users)
			{
				if (user.UserName != "System" && user.UserName != "Administrator")
				{
					var model = new SynchUserViewModel
					{
						Details = user.Comments,
						Email = user.AdditionalInfo.Email,
						FirstName = user.AdditionalInfo.FirstName,
						LastName = user.AdditionalInfo.LastName,
						BirthDate = user.AdditionalInfo.DateOfBirth,
						Phone = user.AdditionalInfo.LandlinePhone + " / " + user.AdditionalInfo.MobilePhone
					};
					if (user.Roles.Count > 0) model.RoleId = user.Roles.FirstOrDefault()?.Name;
					model.Username = user.UserName;
					if (user.AdditionalInfo.Gender == null) model.Gender = 1; else model.Gender = (int)user.AdditionalInfo.Gender.Id;
					if (user.Regions != null)
					{
						var regions = user.Regions.Select(userRegion => (int) userRegion.Id).Select(dummy => (long) dummy).ToList();
						model.Regions = regions;
					}
					list.Add(model);
				}
			}
			var myContent = JsonConvert.SerializeObject(list, Formatting.Indented);
			return myContent;
		}

		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.Authorize]
		public HttpStatusCode SynchAddUserRole([FromBody]SynchUserViewModel model)
		{
			try
			{
				var sessionFactory = IoC.GetSessionFactory();
				var repository = new Repository(sessionFactory);
				var userBll = new SRVUserBll(sessionFactory);
				var userStore = new UserRepository<SRVIdentityUser>(sessionFactory);
				UserManager = new UserManager<SRVIdentityUser>(userStore);

				//** Add user role
				var dto = UserManager.FindByNameAsync(model.Username).Result;
				var roleId = repository.Query<IdentityRole>().Where(x => x.Name.Trim() == model.RoleId.Trim());
				if (roleId != null)
				{
					var userRoles = UserManager.GetRolesAsync(dto.Id).Result;
					if (userRoles.Count > 0)
					{
						foreach (var item in userRoles)
						{
							UserManager.RemoveFromRoleAsync(dto.Id, item);
						}
					}
					var result = UserManager.AddToRoleAsync(dto.Id, model.RoleId.Trim()).Result;
					if (model.RoleId != "Administrator")
					{
						userBll.SetUserRegions(dto.Id, (IEnumerable<long>)model.Regions);
					}
					sessionFactory.GetCurrentSession().Flush();
					return HttpStatusCode.Accepted;
				}
				return HttpStatusCode.BadRequest;
			}
			catch (Exception)
			{
				return HttpStatusCode.BadRequest;
			}
		}

		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.Authorize]
		public HttpStatusCode SynchDeleteUserRole([FromBody]SynchUserViewModel model)
		{
			try
			{
				var sessionFactory = IoC.GetSessionFactory();
				var userBll = new SRVUserBll(sessionFactory);
				var userStore = new UserRepository<SRVIdentityUser>(sessionFactory);
				UserManager = new UserManager<SRVIdentityUser>(userStore);

				//** Add user role
				var dto = UserManager.FindByNameAsync(model.Username).Result;
				var userRoles = UserManager.GetRolesAsync(dto.Id).Result;
				if (userRoles.Count > 0)
				{
					foreach (var item in userRoles)
					{
						UserManager.RemoveFromRoleAsync(dto.Id, item);
					}
				}
				userBll.SetUserRegions(dto.Id, (IEnumerable<long>)model.Regions);
				sessionFactory.GetCurrentSession().Flush();
				return HttpStatusCode.Accepted;
			}
			catch (Exception exception)
			{
				return HttpStatusCode.BadRequest;
			}
		}

		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.Authorize]
		public HttpStatusCode SynchPassword([FromBody]SynchPasswordViewModel model)
		{
			try
			{
				var sessionFactory = IoC.GetSessionFactory();
				var userStore = new UserRepository<SRVIdentityUser>(sessionFactory);
				UserManager = new UserManager<SRVIdentityUser>(userStore);

				//** Change user password
				var dto = UserManager.FindByNameAsync(model.UserId).Result;
				var result = UserManager.ChangePasswordAsync(dto.Id, model.OldPassword, model.NewPassword);
				sessionFactory.GetCurrentSession().Flush();
				return HttpStatusCode.Accepted;
			}
			catch
			{
				return HttpStatusCode.BadRequest;
			}

		}

		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.Authorize]
		public HttpStatusCode SynchRole([FromBody]SynchRoleViewModel model)
		{
			try
			{
				var sessionFactory = IoC.GetSessionFactory();
				var repository = new Repository(sessionFactory);
				var newRole = new IdentityRole
				{
					Name = model.Name
				};
                var exist = repository.Query<IdentityRole>().FirstOrDefault(x => x.Name == model.Name);
                if (exist != null)
                {
                    exist.Name = newRole.Name;
                    repository.SaveOrUpdate(exist);
                }
                else
                {
                    repository.SaveOrUpdate(newRole);
                }
				sessionFactory.GetCurrentSession().Flush();
				return HttpStatusCode.Accepted;
			}
			catch
			{
				return HttpStatusCode.BadRequest;
			}


		}

		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.Authorize]
		public HttpStatusCode SynchUser([FromBody]SynchUserViewModel model)
		{
			try
			{
				var sessionFactory = IoC.GetSessionFactory();
				var repository = new Repository(sessionFactory);
				var userBll = new SRVUserBll(sessionFactory);
				var userStore = new UserRepository<SRVIdentityUser>(sessionFactory);
				UserManager = new UserManager<SRVIdentityUser>(userStore);
				//*** Method
				SRVIdentityUser dto;
				if (model.OldUsername != null)
				{
					dto = UserManager.FindByNameAsync(model.OldUsername).Result;
				}
				else
				{
					dto = UserManager.FindByNameAsync(model.Username).Result;
				}
				if (dto == null)
				{
					var user = new SRVIdentityUser
					{
						UserName = model.Username,
						AdditionalInfo = new AdditionalUserInfo
						{
							FirstName = model.FirstName,
							LastName = model.LastName,
							Email = model.Email,
							Gender = repository.Get<Gender>(model.Gender), // TO CHANGE
							LandlinePhone = model.Phone,
							Created = DateTime.Now,
							DateOfBirth = model.BirthDate,
						},
						Comments = model.Details,
					};
				    if (model.Password == null)
				    {
				        model.Password = Guid.NewGuid().ToString();

				    }
					var result = UserManager.CreateAsync(user, model.Password).Result;
					sessionFactory.GetCurrentSession().Flush();
				}
				else
				{
					userBll.UpdateAccount(dto.Id, null, model.Status == Convert.ToInt64(AccountStatus.Blocked), model.Details);
					var userProfile = userBll.GetUserProfile(dto.Id);
					userProfile.LoginName = model.Username;
					userProfile.FirstName = model.FirstName;
					userProfile.LastName = model.LastName;
					userProfile.Email = model.Email;
					userProfile.LandlinePhone = model.Phone;
					userProfile.DateOfBirth = model.BirthDate;
					userProfile.Gender = repository.Get<Gender>(model.Gender);
					userBll.StoreUserProfile(userProfile);
					if (model.ChangePassword)
					{
						var passwordHash = UserManager.PasswordHasher.HashPassword(model.Password);
						userStore.SetPasswordHashAsync(dto, passwordHash);
					}
					if (model.OldUsername != null)
					{
						var oldUser = repository.Query<SRVIdentityUser>().FirstOrDefault(x => x.UserName == model.OldUsername);
						if (oldUser != null)
						{
							oldUser.UserName = model.Username;
							repository.SaveOrUpdate<SRVIdentityUser>(oldUser);
						}
					}
					sessionFactory.GetCurrentSession().Flush();
				}
				return HttpStatusCode.Accepted;
			}
			catch (Exception exception)
			{
				return HttpStatusCode.BadRequest;
			}
		}

		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.Authorize]
		public HttpStatusCode SynchUserStatus([FromBody]SynchUserViewModel model)
		{
			try
			{
				var sessionFactory = IoC.GetSessionFactory();
				var repository = new Repository(sessionFactory);
				var userBll = new SRVUserBll(sessionFactory);
				var userStore = new UserRepository<SRVIdentityUser>(sessionFactory);
				UserManager = new UserManager<SRVIdentityUser>(userStore);

				//*** Method
				var user = UserManager.FindByNameAsync(model.Username).Result;
				userBll.UpdateAccount(user.Id, user.Roles.FirstOrDefault()?.Id, model.Status == Convert.ToInt64(AccountStatus.Blocked), model.Details);
				sessionFactory.GetCurrentSession().Flush();
				return HttpStatusCode.Accepted;
			}
			catch (Exception exception)
			{
				return HttpStatusCode.BadRequest;
			}
		}

		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.Authorize]
		public HttpStatusCode SyncRoleTransaction([FromBody]SynchTransactionViewModel model)
		{
			try
			{
				var sessionFactory = IoC.GetSessionFactory();
				var repository = new Repository(sessionFactory);
				var roleObject = repository.Query<IdentityRole>().FirstOrDefault(x => x.Name == model.RoleName);
				if (model.Remove)
				{
					foreach (var item in model.Transactions)
					{
						var transactionObject = repository.Query<RoleTransaction>().FirstOrDefault(x => x.Role.Name == model.RoleName && x.Transaction.Code == item);
                        if(transactionObject != null)
                        {
                            repository.Delete<RoleTransaction>(transactionObject);
                        }
					}
					sessionFactory.GetCurrentSession().Flush();
				}
				else
				{
					foreach (var item in model.Transactions)
					{
						var transaction = repository.Query<Transaction>().FirstOrDefault(x => x.Code == item);
						var role = repository.Query<IdentityRole>().FirstOrDefault(x => x.Name == model.RoleName);
						var transactionRelation = repository.Query<RoleTransaction>().FirstOrDefault(x => x.Role == role && x.Transaction == transaction);
						if (transactionRelation == null)
						{
							var transactionObject = repository.Query<Transaction>().FirstOrDefault(x => x.Code == item);
							var roleTransaction = new RoleTransaction
							{
								Transaction = transactionObject,
								Role = roleObject,
							};
							repository.SaveOrUpdateAsync<RoleTransaction>(roleTransaction);
						}
					}
					sessionFactory.GetCurrentSession().Flush();
				}
				return HttpStatusCode.Accepted;
			}
			catch
			{
				return HttpStatusCode.BadRequest;
			}


		}
	}
}