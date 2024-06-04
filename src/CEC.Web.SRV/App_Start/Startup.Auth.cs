using Amdaris.Domain;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Identity;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.Web.SRV.Properties;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using NHibernate;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Configuration;

namespace CEC.Web.SRV
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            var cookieAuthenticationOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieName = "CEC.SRV",
                ExpireTimeSpan = Settings.Default.AuthExpiration,
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ApplyRedirect
                },
            };

            app.UseCookieAuthentication(cookieAuthenticationOptions);
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            GlobalConfiguration.Configuration.MessageHandlers.Add(new BasicAuthenticationMessageHandler());
        }

	    private static void ApplyRedirect(CookieApplyRedirectContext context)
	    {
		    Uri absoluteUri;
		    if (Uri.TryCreate(context.RedirectUri, UriKind.Absolute, out absoluteUri))
		    {
			    var path = PathString.FromUriComponent(absoluteUri);
			    if (path == context.OwinContext.Request.PathBase + context.Options.LoginPath)
			    {
				    var url = ConfigurationManager.AppSettings["Cec.Admin.Url"];

					context.RedirectUri = url +
				                          new QueryString(
					                          context.Options.ReturnUrlParameter,
					                          context.Request.Uri.AbsoluteUri);
			    }
		    }

		    context.Response.Redirect(context.RedirectUri);
	    }
	}

    public class BasicAuthenticationMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sessionFactory = IoC.Resolve<ISessionFactory>();
            var session = new Lazy<ISession>(() => sessionFactory.OpenSession());
            LazySessionContext.Bind(session, sessionFactory);

            HttpContext.Current.Items["NHibernateCurrentSessionFactory"] = new Dictionary<ISessionFactory, Lazy<ISession>>();

            (HttpContext.Current.Items["NHibernateCurrentSessionFactory"] as IDictionary<ISessionFactory, Lazy<ISession>>).Add(sessionFactory, session);

            var _repository = IoC.Resolve<IRepository>();
            var _userBll = new SRVUserBll(sessionFactory);
            var _userStore = new UserRepository<SRVIdentityUser>(sessionFactory);
            var UserManager = new UserManager<SRVIdentityUser>(_userStore);


            var authHeader = request.Headers.Authorization;

            if (authHeader == null) return base.SendAsync(request, cancellationToken);

            if (authHeader.Scheme != "Basic") return base.SendAsync(request, cancellationToken);

            var encodedUserPass = authHeader.Parameter.Trim();
            var userPass = Encoding.ASCII.GetString(Convert.FromBase64String(encodedUserPass));
            var parts = userPass.Split(":".ToCharArray());
            var username = parts[0];
            var password = parts[1];

            var user = UserManager.FindByNameAsync(username).Result;

            HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var identity = UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            HttpContext.Current.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity.Result);

            if (user != null)
            {
                var roles = user.Roles.Select(s => s.Name).ToArray();
                var principal = new ClaimsPrincipal(identity.Result);
                request.GetRequestContext().Principal = principal;
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}