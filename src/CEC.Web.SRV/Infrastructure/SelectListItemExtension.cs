using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.SRV.BLL.Dto;
using CEC.Web.SRV.Models;

namespace CEC.Web.SRV.Infrastructure
{
    public static class SelectListItemExtension
    {
        public static IEnumerable<SelectListItem> ToSelectList<T>(
            this IEnumerable<T> source,
            long selectedId,
            Func<T, long> valueFunc,
            Func<T, string> textFieldFunc,
            SelectListItem allItem)
        {
            var items = ToSelectList(source, selectedId, false, "Select default option", valueFunc, textFieldFunc).ToList();
            items.ForEach(x => x.Selected = false);
            items.Insert(0, allItem);
            return items;
        }

        public static IEnumerable<SelectListItem> ToSelectList<T>(
            this IEnumerable<T> source,
            long selectedId,
            bool useDefaultOption,
            Func<T, long> valueFunc,
            Func<T, string> textFieldFunc)
        {
            return ToSelectList(source, selectedId, useDefaultOption, "Select default option", valueFunc, textFieldFunc);
        }

        public static IEnumerable<SelectListItem> ToSelectListUnEncrypted<T>(
            this IEnumerable<T> source,
            IList<long> selectedIds,
            bool useDefaultOption,
            Func<T, long> valueFunc,
            Func<T, string> textFieldFunc)
        {
            return ToSelectList(source, selectedIds.Select(x => x.ToString()).ToList(), useDefaultOption,
                "Select default option", x => valueFunc(x).ToString(), textFieldFunc);
        }


		public static IEnumerable<Select2Item> ToSelectSelect2List<T>(
		   this IEnumerable<T> source,
		   Func<T, long> valueFunc,
		   Func<T, string> textFieldFunc)
		{
			return source.Select(x => new Select2Item {id = valueFunc(x), text = textFieldFunc(x)});
		}

        //public static IEnumerable<SelectListItem> ToSelectList<T>(
        //    this IEnumerable<T> source,
        //    long selectedId,
        //    bool useDefaultOption,
        //    Func<T, string> textFieldFunc)
        //    where T : LookupDto
        //{
        //    return ToSelectList(source, selectedId, useDefaultOption, "Select default option", x => x.Id, textFieldFunc);
        //}

        //public static IEnumerable<SelectListItem> ToSelectList<T>(
        //    this IEnumerable<T> source,
        //    long selectedId, Func<T,
        //    string> textFieldFunc,
        //    SelectListItem allItem)
        //    where T : LookupDto
        //{
        //    var items = ToSelectList(source, selectedId, false, "Select default option", x => x.Id, textFieldFunc).ToList();
        //    items.ForEach(x => x.Selected = false);
        //    items.Insert(0, allItem);
        //    return items;
        //}

        //public static IEnumerable<SelectListItem> ToSelectListUnEncrypted<T>(
        //    this IEnumerable<T> source,
        //    IList<long> selectedIds,
        //    bool useDefaultOption,
        //    Func<T, string> textFieldFunc)
        //    where T : LookupDto
        //{
        //    return ToSelectList(source, selectedIds, useDefaultOption, "Select default option", x => x.Id.ToString(),
        //        textFieldFunc, false);
        //}

        public static IEnumerable<SelectListItem> ToSelectList<T>(
            this IEnumerable<T> source,
            long selectedId,
            bool useDefaultOption,
            string defaultOptionText,
            Func<T, long> valueFunc,
            Func<T, string> textFieldFunc)
        {
            return ToSelectList(source, new List<string> { selectedId.ToString() }, useDefaultOption, defaultOptionText,
                         x => valueFunc(x).ToString(), textFieldFunc);
        }

        //public static IEnumerable<SelectListItem> ToSelectListUnencrypted<T>(this IEnumerable<T> source,
        //    long selectedId,
        //    bool useDefaultOption,
        //    Func<T, string> textFieldFunc) where T : LookupDto
        //{
        //    return ToSelectListUnencrypted(source, selectedId, useDefaultOption, "Select default option", textFieldFunc);
        //}

        //public static IEnumerable<SelectListItem> ToSelectListUnencrypted<T>(this IEnumerable<T> source,
        //    long selectedId,
        //    bool useDefaultOption,
        //    string defaultOptionText,
        //    Func<T, string> textFieldFunc) where T : LookupDto
        //{
        //    var selectListData = new List<SelectListItem>();
        //    if (useDefaultOption)
        //    {
        //        selectListData.Add(new SelectListItem
        //        {
        //            Value = string.Empty,
        //            Text = defaultOptionText,
        //            Selected = selectedId == 0
        //        });
        //    }

        //    selectListData.AddRange(from s in source
        //                            select new SelectListItem
        //                            {
        //                                Value = s.Id.ToString(),
        //                                Text = textFieldFunc.Invoke(s),
        //                                Selected = s.Id == selectedId
        //                            });
        //    return selectListData;
        //}

        public static IEnumerable<SelectListItem> ToSelectListUnencrypted<T>(this IEnumerable<T> source,
            long selectedId, bool useDefaultOption,
            string defaultOptionText, Func<T, string> textFieldFunc, Func<T, long> valueFieldFunc)
        {
            return ToSelectList(
                source,
                new List<string> { selectedId.ToString() }, useDefaultOption, defaultOptionText,
                    x => valueFieldFunc(x).ToString(), textFieldFunc);
        }

        public static IEnumerable<SelectListItem> ToSelectListUnencrypted<T>(this IEnumerable<T> source,
            long selectedId, bool useDefaultOption,
            string defaultOptionText, Func<T, string> textFieldFunc, Func<T, string> valueFieldFunc)
        {
            return ToSelectList(
                source,
                new List<string> { selectedId.ToString() }, useDefaultOption, defaultOptionText,
                    valueFieldFunc, textFieldFunc);
        }

        public static IEnumerable<SelectListItem> ToSelectList(this IDictionary<string, string> sourceDictionary, string selectedValue = null)
        {
            return ToSelectList(sourceDictionary.Keys, new List<string> {selectedValue}, false, null, x => x,
                x => sourceDictionary[x]);
        }

        public static IEnumerable<SelectListItem> ToSelectList<T>(
		  this IEnumerable<T> source,
		  IList<string> selectedIds,
		  bool useDefaultOption,
		  string defaultOptionText,
		  Func<T, string> valueFunc,
		  Func<T, string> textFieldFunc)
		{
			if (source == null)
				return new List<SelectListItem>();

			var selectListData = new List<SelectListItem>();
			if (useDefaultOption)
			{
				selectListData.Add(new SelectListItem
				{
					Value = string.Empty,
					Text = defaultOptionText
				});
			}

			selectListData.AddRange(from s in source
									select new SelectListItem
									{
										Value = valueFunc(s),
										Text = textFieldFunc(s),
									});

			selectListData.ForEach(x => x.Selected = selectedIds != null &&
										selectedIds.Count != 0 &&
										selectedIds.Any(z => !string.IsNullOrEmpty(x.Value) && z.ToString() == x.Value));

			return selectListData;
		}
    }
}