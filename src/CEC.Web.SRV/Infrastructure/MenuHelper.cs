using Amdaris;
using CEC.Web.SRV.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;

namespace CEC.Web.SRV.Infrastructure
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
            
            var requestController = helper.ViewContext.RouteData.Values["controller"];
            var requestAction = helper.ViewContext.RouteData.Values["action"];
            foreach (var item in accessibleMenu)
            {
                item.IsActive = false;
                foreach (var menuItem1 in item.SubItems)
                {
                    menuItem1.IsActive = false;
                    if (menuItem1.Action == (string) requestAction &&
                        menuItem1.Controller == (string) requestController)
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
            lock (o)
            {
                accessibleMenu = ProcessItems(helper, CreateFullMenu());
                var cacheItem = new MenuCacheItem { CultureName = currentCulture.Name, Menu = accessibleMenu };
                _menuCache[userName] = cacheItem;
            }

            return accessibleMenu;
        }

        private static List<MenuItem> CreateFullMenu()
        {
			 List<MenuItem> list = new List<MenuItem>();
	        if (SecurityHelper.LoggedUserIsInRole(Transactions.Home))
	        {
		        list.Add(new MenuItem
		        {
			        Title = MUI.mnuHome,
			        Css = "fa fa-home",
			        IsActive = true,
			        Controller = "Home",
			        Action = "Index"
		        });

			}
	        if (SecurityHelper.LoggedUserIsInRole(Transactions.Account))
	        {
		        list.Add(new MenuItem
		        {
			        Title = MUI.UsersRoles,
			        Css = "fa fa-pencil",
			        SubItems = new List<MenuItem>
			        {
				        new MenuItem {Title = MUI.CreateUser, Controller = "Account", Action = "CreateUser"},
				        new MenuItem {Title = MUI.Users, Controller = "Account", Action = "Users"},
			        }
		        });

	        }
	        if (SecurityHelper.LoggedUserIsInRole(Transactions.Voters))
	        {
		        list.Add(new MenuItem
		        {
			        Title = MUI.VotersMgmt,
			        Css = "fa fa-pencil-square-o",
			        SubItems = new List<MenuItem>
			        {
				        new MenuItem {Title = MUI.FullList, Controller = "Voters", Action = "Index"},
				        new MenuItem {Title = MUI.StayStatements, Controller = "Voters", Action = "StayStatements"},
				        new MenuItem {Title = MUI.ByBuildings},
				        new MenuItem {Title = MUI.ByExclusions},
			           // new MenuItem {Title = MUI.Voters_VoterProfile, Controller = "Voters", Action = "VoterProfile" },
                    }
		        });

	        }
	        if (SecurityHelper.LoggedUserIsInRole(Transactions.CheckRSP))
	        {
		        list.Add(new MenuItem { Title = MUI.RspChecking, Css = "fa fa-search", Controller = "Voters", Action = "RspChecking" });
			}
			if (SecurityHelper.LoggedUserIsInRole(Transactions.Reporting))
			{

                //list.Add(new MenuItem
                //{
                //    Title = MUI.mnuReports,
                //    Css = "fa fa-list",
                //    IsActive = true,
                //    Controller = "Reporting",
                //    Action = "Index"
                //});

                list.Add(new MenuItem
                {
                    Title = MUI.mnuReports,
                    Css = "fa fa-list",
                    SubItems = new List<MenuItem>
                    {
                        new MenuItem {Title = MUI.PollingStationsList},
                        new MenuItem {Title = MUI.StreetsList},
                        new MenuItem {Title = MUI.PollingSectorsBorders},
                        new MenuItem {Title = MUI.StayStatements},
                        new MenuItem {Title = MUI.mnuPrinting, Controller = "Reporting", Action = "ListPrinting"},
                        new MenuItem {Title = MUI.mnuReport_PSBorders, Controller = "Reporting", Action = "PollingStationsBorders"},
                        new MenuItem {Title = "Rapoarte adiționale", Controller = "Reporting", Action = "Index"},
                    }
                });
            }
	        if (SecurityHelper.LoggedUserIsInRole(Transactions.Lookup))
			{

				list.Add(new MenuItem
				{
					Title = MUI.Lookups,
					Css = "fa fa-dashboard",
					SubItems = new List<MenuItem>
					{
						new MenuItem {Title = MUI.ManagerTypes, Controller = "Lookup", Action = "ManagerTypes"},
						new MenuItem {Title = MUI.PersonStatus, Controller = "Lookup", Action = "PersonStatus"},
						new MenuItem {Title = MUI.Genders_TabTitle, Controller = "Lookup", Action = "Genders"},
						new MenuItem {Title = MUI.DocumentTypes, Controller = "Lookup", Action = "DocumentTypes"},
						new MenuItem {Title = MUI.mnuElectionTypes, Controller = "Lookup", Action = "ElectionTypes"},
						new MenuItem {Title = MUI.mnuElections, Controller = "Election", Action = "Index"},
						new MenuItem {Title = MUI.PersonAddressTypes_TabTitle, Controller = "Lookup", Action = "PersonAddressTypes"},
						new MenuItem {Title = MUI.LocalityTypes, Controller = "Lookup", Action = "RegionTypes"},
						new MenuItem
						{
							Title = MUI.mnuRegions,
							Css = "fa fa-plus-square",
							SubItems = new List<MenuItem>
							{
								new MenuItem {Title = MUI.mnuTreeView, Controller = "Lookup", Action = "RegionsTree"},
								new MenuItem {Title = MUI.mnuGridView, Controller = "Lookup", Action = "RegionsGrid"},
							}
						},
						new MenuItem {Title = MUI.mnuRegions, Controller = "Lookup", Action = "RegionsTable"},
						new MenuItem {Title = MUI.StreetTypes, Controller = "Lookup", Action = "StreetTypes"},
						new MenuItem {Title = MUI.StreetRSPClassifier, Controller = "Lookup", Action = "StreetRspTypes"},
						new MenuItem {Title = MUI.Circumscription, Controller = "Lookup", Action = "Circumscriptions"},

						new MenuItem {Title = MUI.Streets, Controller = "Lookup", Action = "Streets"},
						new MenuItem {Title = MUI.Buildings, Controller = "Address", Action = "Buildings"},
						new MenuItem {Title = MUI.PollingStations, Controller = "PollingStation", Action = "Index"},
					    new MenuItem {Title = MUI.ConflictShareReasons, Controller = "Lookup", Action = "ConflictShareReasons" },

                        new MenuItem {Title = MUI.LocalityInfo},
					}

				});
			}

	        if (SecurityHelper.LoggedUserIsInRole(Transactions.LookupRegister))
	        {
			        list.Add(new MenuItem
			        {
				        Title = MUI.Lookups,
				        Css = "fa fa-dashboard",
				        IsActive = true,
						SubItems = new List<MenuItem>
				        {
					        new MenuItem {Title = MUI.Streets, Controller = "Lookup", Action = "Streets"},
							new MenuItem {Title = MUI.Buildings, Controller = "Address", Action = "Buildings"},
							new MenuItem {Title = MUI.PollingStations, Controller = "PollingStation", Action = "Index"},
						}

			        });

			}

	        if (SecurityHelper.LoggedUserIsInRole(Transactions.Notification))
			{
				list.Add(new MenuItem { Title = MUI.Notification, Css = "fa fa-envelope-o", Controller = "Notification", Action = "Index" });
			}
	        if (SecurityHelper.LoggedUserIsInRole(Transactions.Conflict))
			{
				list.Add(new MenuItem
				{
					Title = MUI.ConflictMgmt,
					Css = "fa fa-ambulance",
					SubItems = new List<MenuItem>
					{
						new MenuItem {Title = MUI.Conflict_Resolution, Controller = "Conflict", Action = "Index"},
						new MenuItem {Title = MUI.RSPAddressMappings, Controller = "Address", Action = "RspAddressMappings"},
					}
				});
			}
	        if (SecurityHelper.LoggedUserIsInRole(Transactions.QuartzAdmin))
	        {
				list.Add(new MenuItem { Title = MUI.QuartzAdmin, Css = "fa fa-gears", Controller = "QuartzAdmin", Action = "Index" });
	        }
	        if (SecurityHelper.LoggedUserIsInRole(Transactions.Export))
			{
				list.Add(new MenuItem
				{
					Title = MUI.mnuExport,
					Css = "fa fa-upload",
					SubItems = new List<MenuItem>
					{
						new MenuItem {Title = MUI.ExportList, Controller = "Export", Action = "ExportList"},
                        new MenuItem {Title = MUI.ExportElectionResult, Controller = "Export", Action = "ExportElectionResult"},
                        new MenuItem {Title = MUI.ExportRsaToSaise, Controller = "Export", Action = "ExportRsaToSaise"},
						new MenuItem {Title = MUI.HistoryExportList, Controller = "Export", Action = "HistoryExportList"},
						new MenuItem {Title = MUI.HistoryExportRsaToSaise, Controller = "Export", Action = "HistoryExportRsaToSaise"},
					}
				});
			}
	        if (SecurityHelper.LoggedUserIsInRole(Transactions.Interop))
	        {
	            list.Add(new MenuItem
	            {
	                Title = MUI.mnuInterop,
	                Css = "fa fa-gears",
	                SubItems = new List<MenuItem>
	                {
	                    //new MenuItem {Title = MUI.mnuInterop_Dashboard, Controller = "Interop", Action = "Dashboard", Css = "fa fa-dashboard"},
	                    new MenuItem {Title = MUI.mnuInterop_Systems, Controller = "Interop", Action = "InteropSystems", Css = "fa fa-list"},
	                    new MenuItem {Title = MUI.mnuInterop_Institutions, Controller = "Interop", Action = "Institutions", Css = "fa fa-list"},
	                    new MenuItem {Title = MUI.mnuInterop_Transactions, Controller = "Interop", Action = "Transactions", Css = "fa fa-list"},
	                }
                });
            }
			if (SecurityHelper.LoggedUserIsInRole(Transactions.AppConfiguration))
			{
				list.Add(new MenuItem
				{
					Title = MUI.mnuApp_Settings,
					Css = "fa fa-cog",
					SubItems = new List<MenuItem>
					{
						new MenuItem{Title = MUI.Config_SettingsManager, Controller = "AppConfiguration", Action = "Index"}
					}
				});
			}


	        return list;
        }

        private static List<MenuItem> ProcessItems(HtmlHelper helper, IEnumerable<MenuItem> items)
        {
            var newMenu = new List<MenuItem>();
            foreach (var menuItem in items)
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