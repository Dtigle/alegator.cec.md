using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Microsoft.Web.Mvc;

namespace CEC.Web.SRV.Infrastructure
{
    public static class LinkExtensions
    {
        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string moduleName = null)
        {
            return SecureActionLink(htmlHelper, moduleName, linkText, actionName, null /* controllerName */, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string moduleName, string linkText, string actionName, object routeValues)
        {
            return SecureActionLink(htmlHelper, moduleName, linkText, actionName, null /* controllerName */, new RouteValueDictionary(routeValues), new RouteValueDictionary());
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string moduleName, string linkText, string actionName, object routeValues, object htmlAttributes)
        {
            return SecureActionLink(htmlHelper, moduleName, linkText, actionName, null /* controllerName */, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string moduleName, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return SecureActionLink(htmlHelper, moduleName, linkText, actionName, null /* controllerName */, routeValues, new RouteValueDictionary());
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string moduleName, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return SecureActionLink(htmlHelper, moduleName, linkText, actionName, null /* controllerName */, routeValues, htmlAttributes);
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string moduleName, string linkText, string actionName, string controllerName)
        {
            return SecureActionLink(htmlHelper, moduleName, linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string moduleName, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, bool setDelimiter = false)
        {
            return SecureActionLink(htmlHelper, moduleName, linkText, actionName, controllerName, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes), setDelimiter);
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string moduleName, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool setDelimiter = false)
        {
            if (String.IsNullOrEmpty(linkText))
            {
                throw new ArgumentException("linkText");
            }

	        string areaName = null;
			if (!string.IsNullOrWhiteSpace(moduleName))
	        {
		        routeValues = routeValues ?? new RouteValueDictionary(new {area = moduleName});
			    areaName = routeValues.ContainsKey("area") ? routeValues["area"].ToString() : moduleName;

		        if (!routeValues.ContainsKey("area"))
		        {
			        routeValues.Add("area", areaName);
		        }
	        }

	        if (IsActionLinkAccessibile(htmlHelper, areaName, controllerName, actionName))
            {
                var res = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
                if (setDelimiter)
                {
                    return MvcHtmlString.Create(@"<br/>" + res);
                }
                return res;
            }

            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string moduleName, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
        {
            return SecureActionLink(htmlHelper, moduleName, linkText, actionName, controllerName, protocol, hostName, fragment, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string moduleName, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            if (String.IsNullOrEmpty(linkText))
            {
                throw new ArgumentException("linkText");
            }

            if (IsActionLinkAccessibile(htmlHelper, moduleName, controllerName, actionName))
            {
                return htmlHelper.ActionLink(
                    linkText, actionName, controllerName, protocol, hostName, fragment, routeValues, htmlAttributes);
            }

            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString SecureActionLink<TController>(this HtmlHelper helper, Expression<Action<TController>> action, string moduleName, string linkText) where TController : Controller
        {
            return SecureActionLink(helper, action, moduleName, linkText, null, false);
        }

        public static MvcHtmlString SecureActionLink<TController>(this HtmlHelper helper, Expression<Action<TController>> action, string moduleName, string linkText, object htmlAttributes, bool setDelimiter = false) where TController : Controller
        {
            var controllerName = typeof(TController).Name.Replace("Controller", string.Empty);
            var callExpression = action.Body as MethodCallExpression;

            if (callExpression == null)
            {
                return helper.ActionLink(action, linkText, htmlAttributes);
            }

            string actionName = callExpression.Method.Name;
            if (IsActionLinkAccessibile(helper, moduleName, controllerName, actionName))
            {
                var ret = helper.ActionLink(action, linkText, htmlAttributes);
                return setDelimiter ? MvcHtmlString.Create(@"<br/>" + ret) : ret;
            }

            return MvcHtmlString.Empty;
        }

        private static bool IsActionLinkAccessibile(HtmlHelper helper, string moduleName, string controllerName, string actionName)
        {
            var viewContext = helper.ViewContext;
            controllerName = controllerName ?? viewContext.RouteData.GetRequiredString("controller");
            actionName = actionName ?? viewContext.RouteData.GetRequiredString("action");

            return helper.HasActionPermission(actionName, controllerName);
        }
    }
}