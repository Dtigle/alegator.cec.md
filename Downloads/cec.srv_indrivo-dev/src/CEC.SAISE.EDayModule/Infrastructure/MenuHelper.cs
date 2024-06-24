using Amdaris;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CEC.SAISE.EDayModule.Infrastructure
{
    public static class MenuHelper
    {
        private static readonly Dictionary<string, MenuCacheItem> _menuCache =
            new Dictionary<string, MenuCacheItem>();

        public static void ClearCache()
        {
            var userName = HttpContext.Current.User.Identity.Name;
            lock (userName)
            {
                _menuCache.Remove(userName);
            }
        }

        public static string ActionLink(this MenuItem item, UrlHelper helper)
        {
            if (!string.IsNullOrWhiteSpace(item.Action) && !string.IsNullOrWhiteSpace(item.Controller))
            {
                return helper.Action(item.Action, item.Controller);
            }

            return "#";
        }

        public static IEnumerable<MenuItem> GetMenuItems(HtmlHelper helper)
        {
            var userName = HttpContext.Current.User.Identity.Name;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            MenuCacheItem cacheItem;
            IEnumerable<MenuItem> accessibleMenu = null;
            lock (userName)
            {
                if (!_menuCache.TryGetValue(userName, out cacheItem))
                {
                    accessibleMenu = CreateNewMenuAndPutInCache(userName, currentCulture, helper);
                }
                else
                {
                    accessibleMenu = cacheItem.CultureName != currentCulture.Name
                        ? CreateNewMenuAndPutInCache(userName, currentCulture, helper)
                        : cacheItem.Menu;
                }
            }

            var requestController = helper.ViewContext.RouteData.Values["controller"];
            var requestAction = helper.ViewContext.RouteData.Values["action"];
            foreach (var item in accessibleMenu)
            {
                item.IsActive = false;
                foreach (var menuItem1 in item.SubItems)
                {
                    menuItem1.IsActive = false;
                    if (menuItem1.Action == (string)requestAction &&
                        menuItem1.Controller == (string)requestController)
                    {
                        menuItem1.IsActive = true;
                        item.IsActive = true;
                    }
                    else
                    {
                        bool isActivNode = false;
                        SetActiveMenu(menuItem1, requestController, requestAction, out isActivNode);
                        if (isActivNode)
                            item.IsActive = true;
                    }
                }
            }
            var menuItemNonCategory = accessibleMenu.FirstOrDefault(x => x.Action == (string)requestAction && x.Controller == (string)requestController);

            if (menuItemNonCategory != null)
            {
                menuItemNonCategory.IsActive = true;
            }


            return accessibleMenu;
        }

        private static void SetActiveMenu(MenuItem menuItem, object requestController, object requestAction, out bool isActive)
        {
            menuItem.IsActive = false;
            foreach (var menuItem2 in menuItem.SubItems)
            {
                menuItem2.IsActive = false;
                if (menuItem2.Action == (string)requestAction &&
                    menuItem2.Controller == (string)requestController)
                {
                    menuItem2.IsActive = true;
                    menuItem.IsActive = true;
                }
                else
                {
                    bool isActiveNode = false;
                    SetActiveMenu(menuItem2, requestController, requestAction, out isActiveNode);
                    if (isActiveNode)
                        menuItem.IsActive = true;
                }
            }
            isActive = menuItem.IsActive;
        }

        private static IEnumerable<MenuItem> CreateNewMenuAndPutInCache(string userName, CultureInfo currentCulture, HtmlHelper helper)
        {
            object o = new object();
            IEnumerable<MenuItem> accessibleMenu = null;
            lock (userName)
            {
                accessibleMenu = ProcessItems(helper, CreateFullMenu());
                var cacheItem = new MenuCacheItem { CultureName = currentCulture.Name, Menu = accessibleMenu };
                _menuCache[userName] = cacheItem;
            }

            return accessibleMenu;
        }

        private static List<MenuItem> CreateFullMenu()
        {
            string allowSynchService = "0";
            string allowReportingService = "0";
            try
            {
                allowSynchService = ConfigurationManager.AppSettings["allowSynchService"];
                allowReportingService = ConfigurationManager.AppSettings["allowReportingService"]; 

            }
            catch 
            {
            }

            List<MenuItem> listMenuItem = new List<MenuItem>
            {
                new MenuItem
                {
                    Title = "Principala",
                    Css = "fa fa-home",
                    IsActive = true,
                    Controller = "Home",
                    Action = "Index"
                },
                allowSynchService == "1" ? new MenuItem { Title = "Ajustare date", Css = "fa fa-refresh", Controller = "Sync", Action = "Index" }: null,
                new MenuItem { Title = "Transfer date BD Raportare", Css = "fa fa-share", Controller = "Transfer", Action = "Index" } ,
                new MenuItem { Title = "Deschidere", Css = "fa fa-clock-o", Controller = "Voting", Action = "OpenPollingStation" },
                new MenuItem { Title = "Prezența la vot", Css = "fa fa-check-square-o", Controller = "Voting", Action = "Index" },
                new MenuItem { Title = "Eliberarea Certificat", Css = "fa fa-certificate", Controller = "VoterCertificat", Action = "Index" },
                //new MenuItem { Title = "Registrarea partid Politic", Css = "fa fa-pencil-square-o", Controller = "RegisterPoliticalParty", Action = "Index" },
                new MenuItem { Title = "Gestionarea concurenților electorali", Css = "fa fa-pencil-square-o", Controller = "PoliticalParty", Action = "Index" },
                new MenuItem { Title = "Documente electorale", Css = "fa fa-pencil-square-o", Controller = "Documents", Action = "Index", IsActive = true },
                new MenuItem { Title = "Gestionarea documentelor electorale", Css = "fa fa-pencil-square-o", Controller = "DocumentsGrid", Action = "Index", IsActive = true },
                new MenuItem { Title = "Gestionarea funcțională", Css = "fa fa-pencil-square-o", Controller = "PermissionManage", Action = "Index" },
                new MenuItem { Title = "Statistici", Css = "fa fa-pencil-square-o", Controller = "VotingProcessStats", Action = "Index" },
                allowReportingService == "1" ? new MenuItem { Title = "Rapoarte", Css = "fa fa fa-list", Controller = "Reporting", Action = "Index" }: null,
                //new MenuItem { Title = "Procesarea Documentelor", Css = "fa fa-file-signature", Controller = "TemplateName", Action = "Index" },
            };

            return listMenuItem;
        }

        private static List<MenuItem> ProcessItems(HtmlHelper helper, IEnumerable<MenuItem> items)
        {
            var newMenu = new List<MenuItem>();
            foreach (var menuItem in items)
            {
                if (menuItem != null)
                {
                                    if (!menuItem.IsCategory)
                {
                    if (!string.IsNullOrWhiteSpace(menuItem.Action) &&
                        !string.IsNullOrWhiteSpace(menuItem.Controller)
                        && HasActionPermission(helper, menuItem.Action, menuItem.Controller))
                    {
                        newMenu.Add(menuItem);
                    }
                }
                else
                {
                    var subItems = ProcessItems(helper, menuItem.SubItems);

                    if (subItems.Count > 0)
                    {

                        newMenu.Add(new MenuItem
                        {
                            Action = menuItem.Action,
                            Controller = menuItem.Controller,
                            Css = menuItem.Css,
                            IsActive = menuItem.IsActive,
                            Title = menuItem.Title,
                            SubItems = subItems
                        });
                    }
                }
                }

            }

            return newMenu;
        }

        public static bool HasActionPermission(this ControllerContext controllerContext, string action, string controller = null)
        {
            try
            {
                //if the controller name is empty the ASP.NET convention is:
                //"we are linking to a different controller
                ControllerBase controllerToLinkTo = string.IsNullOrEmpty(controller)
                                                        ? controllerContext.Controller
                                                        : GetControllerByName(controllerContext.RequestContext, controller);
                try
                {
                    return HasActionPermission(controllerContext.RequestContext, controllerToLinkTo, action);
                }
                finally
                {
                    if (controllerContext.Controller != controllerToLinkTo)
                    {
                        // release controller to avoid memory leaks
                        ControllerBuilder.Current.GetControllerFactory().ReleaseController(controllerToLinkTo);
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Debug(action + "-" + controller + "-" + area, ex);
                Amdaris.DependencyResolver.Current.Resolve<ILogger>().Error(ex, action + "-" + controller);
            }

            return false;
        }

        /// <summary>
        /// Returns true if a specific controller action exists and
        /// the user has the ability to access it.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public static bool HasActionPermission(this HtmlHelper htmlHelper, string action, string controller = null)
        {
            return htmlHelper.ViewContext.HasActionPermission(action, controller);
        }

        private static bool HasActionPermission(RequestContext requestContext, ControllerBase controller, string actionName)
        {
            var controllerContextInAction = new ControllerContext(requestContext, controller);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            var actionDescriptor = controllerDescriptor.FindAction(controllerContextInAction, actionName);
            return ActionIsAuthorized(controllerContextInAction, actionDescriptor);
        }

        private static bool ActionIsAuthorized(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor == null)
                return false; // action does not exist so say yes - should we authorize this?!

            var authContext = new AuthorizationContext(controllerContext, actionDescriptor);

            // run each auth filter until on fails
            // performance could be improved by some caching
            foreach (IAuthorizationFilter authFilter in FilterProviders.Providers.GetFilters(controllerContext, actionDescriptor)
                .Where(x => x.Instance is IAuthorizationFilter)
                .Select(x => x.Instance))
            {
                if (authFilter is ValidateAntiForgeryTokenAttribute)
                {
                    continue;
                }

                authFilter.OnAuthorization(authContext);

                if (authContext.Result != null)
                    return false;
            }

            return true;
        }

        private static ControllerBase GetControllerByName(RequestContext requestContext, string controllerName)
        {
            // Instantiate the controller and call Execute
            var factory = ControllerBuilder.Current.GetControllerFactory();
            var namespaces = requestContext.RouteData.DataTokens["Namespaces"];
            try
            {
                IController controller = factory.CreateController(requestContext, controllerName);
                if (controller == null)
                {
                    throw new InvalidOperationException(

                        String.Format(
                            CultureInfo.CurrentUICulture,
                            "Controller factory {0} controller {1} returned null",
                            factory.GetType(),
                            controllerName));

                }

                return (ControllerBase)controller;
            }
            finally
            {
                requestContext.RouteData.DataTokens["Namespaces"] = namespaces;
            }
        }
    }
}