using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Account;
using CEC.Web.SRV.Resources;
using FluentNHibernate.Conventions;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Amdaris.NHibernateProvider.Identity;
using NHibernate;
using Lib.Web.Mvc.JQuery.JqGrid;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.Web.SRV.Infrastructure.Logger;
using CEC.Web.SRV.LoggingService;

namespace CEC.Web.SRV.Controllers
{

    //todo: remove IRepository 
    public class AccountController : BaseController
    {
        private readonly IRepository _repository;
        private readonly ISRVUserBll _userBll;
        private readonly UserRepository<SRVIdentityUser> _userStore;

        public AccountController(ISessionFactory sessionFactory, IRepository repository)
        {
            _repository = repository;
            _userBll = new SRVUserBll(sessionFactory);
            _userStore = new UserRepository<SRVIdentityUser>(sessionFactory);
            UserManager = new UserManager<SRVIdentityUser>(_userStore);
        }

        public UserManager<SRVIdentityUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                SRVIdentityUser user = await UserManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (user.IsBlocked)
                    {
                        ModelState.AddModelError("", MUI.LoginError_BlockedUser);
                        return View(model);
                    }

					if  (user.Roles.Count == 0)
					{
						ModelState.AddModelError("", MUI.LoginError_NotRole);
						return View(model);
					}

                    Task<bool> checkPasswordAsync = UserManager.CheckPasswordAsync(user, model.Password);

                    if (!checkPasswordAsync.Result)
                    {
                        user.FailLogin();
                        _repository.SaveOrUpdate(user);
                        ModelState.AddModelError("", MUI.LoginError_UserNamePassword);
                        return View(model);
                    }

                    await SignInAsync(user, false);

                    user.SuccessLogin();
                    _repository.SaveOrUpdate(user);

                    return RedirectToLocal(returnUrl);


                }
                else
                {
                    ModelState.AddModelError("", MUI.LoginError_UserNamePassword);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



	    //
		// GET: /Account/Register
		[AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userValidation = await UserManager.FindByNameAsync(model.UserName);
                if (userValidation != null)
                {
                    ModelState.AddModelError("UserName", MUI.RegisterErrorExist_UserName);
                }

                if (ModelState.IsValid)
                {
                    var user = new SRVIdentityUser
                    {
                        UserName = model.UserName,
                        AdditionalInfo = new AdditionalUserInfo
                        {
                            FirstName = model.DisplayName,
                            Email = model.PreferredEmail
                        }
                    };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return PartialView("_ChangePasswordPartial", new ChangePasswordViewModel());
        }

        //
        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return PartialView("_ChangePasswordPartial", model);

            var result =
                await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Content(Const.CloseWindowContent);
            }

            ModelState.AddModelError("OldPassword", MUI.ChangePasswordErrorIncorrect_OldPassword);

            // If we got this far, something failed, redisplay form
            return PartialView("_ChangePasswordPartial", model);
        }

        //
        // GET: /Account/ResetPassword
        [Authorize(Roles = Transactions.Administrator)]
        public ActionResult ResetPassword(string userId)
        {
            var model = new ResetPasswordViewModel { UserId = userId };
            return PartialView("_ResetPasswordPassword", model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return PartialView("_ResetPasswordPassword", model);

            var user = await UserManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ModelState.AddModelError("", "Specified user not found.");
                return PartialView("_ResetPasswordPassword", model);
            }
            var passwordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);

            await _userStore.SetPasswordHashAsync(user, passwordHash);

            return Content(Const.CloseWindowContent);
        }

        ////
        //// POST: /Account/LogOff
        public ActionResult LogOff()
        {
            LoggerUtils logEvent = new LoggerUtils();
            logEvent.LogEvent(LogLevel.Information, Events.Authorisation.Value, Events.Authorisation.Description, new Dictionary<string, string> { { Events.Authorisation.Attributes.Action, "Logout" }, });

            MenuHelper.ClearCache();
            AuthenticationManager.SignOut();
            _userBll.SetAccountLogoutTime();

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = Transactions.Account)]
        public ActionResult Users()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = Transactions.Account)]
        public ActionResult CreateUser()
        {
            SetViewData();
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(CreateUserViewModel model)
        {
            SetViewData();

            var userValidation = await UserManager.FindByNameAsync(model.UserName);
            if (userValidation != null)
            {
                ModelState.AddModelError("UserName", MUI.CreateUserErrorExist_UserName);
            }

            var role = _repository.Get<IdentityRole, string>(model.RoleId);
            if (role == null)
            {
                ModelState.AddModelError("RoleId", MUI.CreateUserErrorExist_UserName);
            }

            if (ModelState.IsValid)
            {
                var user = new SRVIdentityUser
                {
                    UserName = model.UserName,
                    AdditionalInfo = new AdditionalUserInfo
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
						Gender = _repository.Get<Gender>(model.GenderId),
                    },
                    Comments = model.Comments,
                };
                user.Roles.Add(role);
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users", "Account");
                }

                AddErrors(result);
            }
            return View(model);
        }

        private void SetViewData()
        {
	        var gender = _repository.Query<Gender>();
			ViewData["GenderId"] = gender.ToSelectListUnencrypted(0, false, MUI.SelectPrompt, x => x.Name, x => x.Id);

            var roles = _repository.Query<IdentityRole>();
            ViewData["RoleId"] = roles.ToSelectListUnencrypted(0, false, MUI.SelectPrompt, x => x.Name, x => x.Id);
        }

        [HttpPost]
        public JqGridJsonResult ListUsersAjax(JqGridRequest request)
        {
            var pageRequest = request.ToPageRequest<UserGridModel>();

            var list = _userBll.ListUsers(pageRequest);

            return list.ToJqGridJsonResult<SRVIdentityUser, UserGridModel>();
        }

        [HttpPost]
        public ActionResult ExportUsers(JqGridRequest request, ExportType exportType)
        {
            return ExportGridData(request, exportType, "Users", typeof (UserGridModel), ListUsersAjax);
        }

        [HttpGet]
		public ActionResult UpdateUser(string userId)
		{
			var user = _userBll.Get<SRVIdentityUser, string>(userId);
			var rol = user.Roles.Select(x => x.Id).FirstOrDefault();

			var model = new UpdateUserModel
			{
				Id = user.Id,
				Comments = user.Comments,
				RoleId = Convert.ToInt64(rol),
				StatusId = user.IsBlocked ? Convert.ToInt64(AccountStatus.Blocked) : Convert.ToInt64(AccountStatus.Active)
			};

			var roles = _repository.Query<IdentityRole>();
			ViewData["RoleId"] = roles.ToSelectListUnencrypted(model.RoleId, false, null, x => x.Name, x => x.Id);

			var statuses = GetStatusOptions();
			ViewData["StatusId"] = statuses.ToSelectListUnencrypted(model.StatusId, false, null, x => x.Text, x => x.Value);

			return PartialView("_UpdateUserPartial", model);
		}


		[HttpPost]
		public ActionResult UpdateUser(UpdateUserModel model)
		{
			var roles = _repository.Query<IdentityRole>();
			ViewData["RoleId"] = roles.ToSelectListUnencrypted(model.RoleId, false, null, x => x.Name, x => x.Id);

			var statuses = GetStatusOptions();
			ViewData["StatusId"] = statuses.ToSelectListUnencrypted(model.StatusId, false, null, x => x.Text, x => x.Value);

			if (!TryValidateModel(model)) return PartialView("_UpdateUserPartial", model);
			try
			{
				_userBll.UpdateAccount(model.Id, model.RoleId.ToString(), model.StatusId == Convert.ToInt64(AccountStatus.Blocked),
					model.Comments);
				return Content(Const.CloseWindowContent);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
			}
			return PartialView("_UpdateUserPartial", model);
		}

	    [Authorize(Roles = Transactions.Account)]
		public ActionResult UpdateAccounts()
        {
            var model = new MultiSelectAccountEdit();
            var statuses = GetStatusOptions();
            ViewData["Status"] = statuses;

            return PartialView("_MultiSelectEdit", model);
        }

        [HttpPost]
        [Authorize(Roles = Transactions.Account)]
		[ValidateAntiForgeryToken]
        public ActionResult UpdateAccounts(MultiSelectAccountEdit model)
        {
            if (model.Status == 0)
            {
                ModelState.AddModelError("Status", MUI.StatusRequired);
            }

            if (ModelState.IsValid)
            {
                _userBll.ChangeStatus(model.Accounts, model.Status == AccountStatus.Blocked, model.Comments);
                return Content(Const.CloseWindowContent);
            }

            return PartialView("_MultiSelectEdit", model);
        }

        [HttpPost]
		public ActionResult DeleteUser(string userId)
        {
			_userBll.DeleteAccount(userId);

			return Content(Const.CloseWindowContent);
        }

		[HttpPost]
        public ActionResult SetRegionsForUser(string userId)
        {
			var user = _userBll.GetRegistratorUser(userId);
	       
            return PartialView("_SetRegions",
                new AllocateRegionModel
                {
                    UserId = userId,
                    AllocatedRegions =
                        user.Regions.Select(x => new SelectListItem { Text = x.GetFullName(), Value = x.Id.ToString() }).ToList()
                });
			// BAUaa
        }

        [HttpPost]
        public ActionResult SetRegions(AllocateRegionModel model)
        {
            try
            {
                _userBll.SetUserRegions(model.UserId, model.AllocatedRegions.Select(x => Convert.ToInt64(x.Value)));
                return Content(Const.CloseWindowContent);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return PartialView("_SetRegions", model);
            }
        }

		public ActionResult SelectRoles()
		{
			var roles = _repository.Query<IdentityRole>();

			return PartialView("_Select", roles.ToSelectListUnencrypted(0, false, "Selectati...", x => x.Name, x => x.Id));
		}

        public ActionResult AccountStatuses()
        {
            var statuses = GetStatusOptions();

            return PartialView("_Select", statuses);
        }

        public ActionResult AccountStatusesForSearch()
        {
            var statuses = Enum.GetValues(typeof(AccountStatus))
                .Cast<AccountStatus>().Select(x => new SelectListItem
                {
                    Value = x.GetFilterValue().ToString(),
                    Text = x.GetEnumDescription(),
                }).ToList();

            return PartialView("_Select", statuses);
        }

        public ActionResult ViewProfile(string userId)
        {
            var id = !string.IsNullOrWhiteSpace(userId) ? userId : User.Identity.GetUserId();
            string user = id;
            var model = GetUserProfile(user);
			var gender = _repository.Query<Gender>();
			ViewData["GenderId"] = gender.ToSelectListUnencrypted(model.GenderId, false, MUI.SelectPrompt, x => x.Name, x => x.Id);
            return View("UserProfile", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(UserProfileModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var dto = _userBll.GetUserProfile(viewModel.UserId);
				dto.Gender = _userBll.Get<Gender>(viewModel.GenderId);
                this.TryUpdateModel(dto);
                _userBll.StoreUserProfile(dto);

                if (SecurityHelper.LoggedUserIsInRole("Administrator"))
                {
                    return RedirectToAction("Users", "Account");
                }

                return RedirectToAction("Index", "Home");
            }
			var gender = _repository.Query<Gender>();
			ViewData["GenderId"] = gender.ToSelectListUnencrypted(viewModel.GenderId, false, MUI.SelectPrompt, x => x.Name, x => x.Id);
            return View("UserProfile", viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers

        private List<SelectListItem> GetStatusOptions()
        {
            return Enum.GetValues(typeof(AccountStatus))
                .Cast<AccountStatus>().Select(x => new SelectListItem
                {
                    Value = ((int)x).ToString(),
                    Text = x.GetEnumDescription(),
                }).ToList();
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(SRVIdentityUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
	        // Add roles
	        var permissions = _repository.Query<RoleTransaction>().Where(x => x.Role.Id == user.Roles.FirstOrDefault().Id).Select(x => x.Transaction.Name);
	        foreach (var item in permissions)
	        {
		        identity.AddClaim(new Claim(ClaimTypes.Role, item));
	        }
			AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
		}

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        public UserProfileModel GetUserProfile(string userId)
        {
            var dto = _userBll.GetUserProfile(userId);
            return new UserProfileModel
            {
                UserId = dto.Id,
                LoginName = dto.LoginName,
                LastLogin = dto.LastLogin.HasValue ? dto.LastLogin.Value.LocalDateTime : (DateTime?)null,
                LoginCreationDate = dto.LoginCreationDate.LocalDateTime,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                LandlinePhone = dto.LandlinePhone,
                MobilePhone = dto.MobilePhone,
                WorkInfo = dto.WorkInfo,
                IsCurrentUser = User.Identity.GetUserId() == userId,
				GenderId = dto.Gender == null? 0: dto.Gender.Id
            };
        }

	    //
	    // GET: /Account/Login
	    [AllowAnonymous]
	    public async Task<ActionResult> ExternLogin(string login)
	    {
		    var decryptedUserName = Encryption.Decrypt(login);
			SRVIdentityUser user = await UserManager.FindByNameAsync(decryptedUserName);
			await SignInAsync(user, false);
			user.SuccessLogin();
			_repository.SaveOrUpdate(user);
	        LoggerUtils logEvent = new LoggerUtils();
	        logEvent.LogEvent(LogLevel.Information, Events.Authorisation.Value, Events.Authorisation.Description, new Dictionary<string, string> { { Events.Authorisation.Attributes.Action, "Login" }, });

            return RedirectToLocal("/Home");
			//return View("UserProfile");
	    }

	}
}