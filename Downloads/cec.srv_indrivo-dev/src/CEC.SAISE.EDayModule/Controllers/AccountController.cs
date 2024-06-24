using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Infrastructure;
using CEC.SAISE.EDayModule.Infrastructure.Security;
using CEC.SAISE.EDayModule.Models.Account;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NHibernate;

namespace CEC.SAISE.EDayModule.Controllers
{
    public class AccountController : BaseDataController
    {
        private readonly IAuditEvents _auditEvents;
        private readonly IUserBll _userBll;
        public AccountController(ISessionFactory sessionFactory, IAuditEvents auditEvents, IUserBll userBll)
        {
            var userStore = new SaiseUserStore(sessionFactory);
            UserManager = new CustomUserManager(userStore);
            _auditEvents = auditEvents;
            _userBll = userBll;
        }

        public CustomUserManager UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
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
                SystemUser user = await UserManager.FindByNameAsync(model.UserName);
                if (user != null && !user.IsDeleted)
                {
                    var validCredentials = await UserManager.CheckPasswordAsync(user, model.Password);

                    if (await UserManager.IsLockedOutAsync(user.Id))
                    {
                        ModelState.AddModelError("", "Utilizatorul este blocat. Contactați CEC pentru detalii.");
                    }
                    else if (!validCredentials)
                    {
                        await UserManager.AccessFailedAsync(user.Id);

                        if (await UserManager.IsLockedOutAsync(user.Id))
                        {
                            ModelState.AddModelError("", "Utilizatorul este blocat. Contactați CEC pentru detalii.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "'Nume utilizator' sau 'Parola' sunt greșite");
                        }
                    }
                    else 
                    {


                        await SignInAsync(user, false);
                        await UserManager.ResetAccessFailedCountAsync(user.Id);

                        user.SuccessLogin();

                        try
                        {
                            string loger = LoggerUtil.GetIpAddress();


                            await _auditEvents.InsertEvents(AuditEventTypeDto.Login.GetEnumDescription(), user,
                                "Autentificare ", loger);
                        }
                        finally
                        {

                        }


                        return RedirectToLocal(returnUrl);
                    }
                }
                else
                {
					ModelState.AddModelError("", "'Nume utilizator' sau 'Parola' sunt greșite");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        ////
        //// POST: /Account/LogOff
        public ActionResult LogOff()
        {
            try
            {
                string loger = LoggerUtil.GetIpAddress();
                var user = _userBll.GetById(User.Identity.GetUserId<long>());

                _auditEvents.InsertEvents(AuditEventTypeDto.Logout.GetEnumDescription(), user, "Iesire din sistem", loger);
            }
            catch (Exception e)
            {

            }


            MenuHelper.ClearCache();
            AuthenticationManager.SignOut();

            //_userBll.SetAccountLogoutTime();
           
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(SystemUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
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
    }
}