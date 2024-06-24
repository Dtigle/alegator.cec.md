using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEC.Web.SRV.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Select2RemoteConfigAttribute : Attribute
    {
        public string PlaceHolder { get; set; }

        public int MinimumInputLength { get; set; }

		public int PageLimit { get; set; }

        public int AjaxQuietMillis { get; set; }

        public string AjaxAction { get; set; }

        public string AjaxController { get; set; }
        
        public string AjaxDataType { get; set; }

        public string AjaxData_JsFuncName { get; set; }

        public string AjaxResults_JsFuncName { get; set; }

		public string AjaxData_ActionInitSelection { get; set; }

		public string AjaxData_ControllerInitSelection { get; set; }

        public Select2RemoteConfigAttribute()
        {
            MinimumInputLength = 3;
            AjaxQuietMillis = 200;
            AjaxDataType = "json";
        }

		public Select2RemoteConfigAttribute(string placeHolder, string action, string controller,
			string ajaxDataType, string ajaxData_JsFuncName, string ajaxResults_JsFuncName)
		{
			PlaceHolder = placeHolder;
			MinimumInputLength = 3;
			AjaxQuietMillis = 200;
			AjaxAction = action;
			AjaxController = controller;
			AjaxDataType = ajaxDataType;
			AjaxData_JsFuncName = ajaxData_JsFuncName;
			AjaxResults_JsFuncName = ajaxResults_JsFuncName;
		}

		public Select2RemoteConfigAttribute(string placeHolder, string action, string controller,
		   string ajaxDataType, string ajaxData_JsFuncName, string ajaxResults_JsFuncName, int pageLimit)
		{
			PlaceHolder = placeHolder;
			AjaxAction = action;
			AjaxController = controller;
			AjaxDataType = ajaxDataType;
			AjaxData_JsFuncName = ajaxData_JsFuncName;
			AjaxResults_JsFuncName = ajaxResults_JsFuncName;
			PageLimit = pageLimit;
		}

		public Select2RemoteConfigAttribute(string placeHolder, string action, string controller,
	   string ajaxDataType, string ajaxData_JsFuncName, string ajaxResults_JsFuncName, string ajaxData_ActionInitSelection, string ajaxData_ControllerInitSelection)
		{
			PlaceHolder = placeHolder;
			MinimumInputLength = 3;
			AjaxQuietMillis = 200;
			AjaxAction = action;
			AjaxController = controller;
			AjaxDataType = ajaxDataType;
			AjaxData_JsFuncName = ajaxData_JsFuncName;
			AjaxResults_JsFuncName = ajaxResults_JsFuncName;
			AjaxData_ActionInitSelection = ajaxData_ActionInitSelection;
			AjaxData_ControllerInitSelection = ajaxData_ControllerInitSelection;
		}

        public Select2RemoteConfigAttribute(string placeHolder, int minimumInputLength,
            int ajaxQuietMillis, string action, string controller,
			string ajaxDataType, string ajaxData_JsFuncName, string ajaxResults_JsFuncName)
        {
            PlaceHolder = placeHolder;
            MinimumInputLength = minimumInputLength;
            AjaxQuietMillis = ajaxQuietMillis;
            AjaxAction = action;
            AjaxController = controller;
            AjaxDataType = ajaxDataType;
            AjaxData_JsFuncName = ajaxData_JsFuncName;
            AjaxResults_JsFuncName = ajaxResults_JsFuncName;
        }
    }
}