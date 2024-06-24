using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Amdaris.Domain.Paging;
using AutoMapper;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Lib.Web.Mvc.JQuery.JqGrid;
using Filter = Amdaris.Domain.Paging.Filter;

namespace CEC.Web.SRV.Infrastructure.Grids
{
    public static class JqGridExtensions
    {
        public static PageRequest ToPageRequest<T>(this JqGridRequest jqGridRequest) where T : class
        {
            var pageRequest = new PageRequest
                                {
                                    PageNumber = jqGridRequest.PageIndex + 1,
                                    PageSize = jqGridRequest.RecordsCount,
                                };

            MapToSortFields<T>(jqGridRequest, pageRequest);

            MapToSingleFilter<T>(jqGridRequest.SearchingFilter, pageRequest);

            MapToMultipleFilters<T>(jqGridRequest.SearchingFilters, pageRequest);

            return pageRequest;
        }

        private static void MapToSortFields<T>(JqGridRequest jqGridRequest, PageRequest pageRequest) where T : class
        {
            if (string.IsNullOrWhiteSpace(jqGridRequest.SortingName)) return;

            Type type = typeof(T);

            var sortProps = jqGridRequest.SortingName.Trim().Split(new []{","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sortProp in sortProps)
            {
                var sortData = sortProp.Trim().Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                var propName = sortData.First();
                var sortOrder = sortData.Skip(1).FirstOrDefault();

                PropertyInfo prop = type.GetProperty(propName);
                if (prop == null) throw new ArgumentNullException("prop");


                string dbFieldName = propName;

                var attribute = prop.GetCustomAttributes(typeof(SearchDataAttribute), false).FirstOrDefault() as SearchDataAttribute;
                if (attribute != null)
                {
                    if (!string.IsNullOrWhiteSpace(attribute.DbName)) dbFieldName = attribute.DbName;
                }

                var ascending = string.IsNullOrWhiteSpace(sortOrder)
                    ? jqGridRequest.SortingOrder == JqGridSortingOrders.Asc
                    : sortOrder == "asc";

                pageRequest.SortFields.Add(new SortField
                {
                    Ascending = ascending,
                    Property = dbFieldName
                });
            }
        }

        private static void MapToMultipleFilters<T>(JqGridRequestSearchingFilters searchingFilters,
            PageRequest pageRequest) where T : class
        {
            if (searchingFilters == null || searchingFilters.Filters == null || searchingFilters.Filters.Count <= 0)
                return;

            if (searchingFilters.GroupingOperator == JqGridSearchGroupingOperators.And)
            {
                var filterGroup = new FilterGroup();
                foreach (JqGridRequestSearchingFilter searchingFilter in searchingFilters.Filters)
                {
                    filterGroup.Filters.Add(ResolveFilter<T>(searchingFilter));
                }
                pageRequest.FilterGroups.Add(filterGroup);
            }
            else
            {
                foreach (JqGridRequestSearchingFilter searchingFilter in searchingFilters.Filters)
                {
                    var filterGroup = new FilterGroup();
                    filterGroup.Filters.Add(ResolveFilter<T>(searchingFilter));
                    pageRequest.FilterGroups.Add(filterGroup);
                }
            }
        }

        private static void MapToSingleFilter<T>(JqGridRequestSearchingFilter searchingFilter, PageRequest pageRequest)
            where T : class
        {
            if (searchingFilter == null) return;

            var filterGroup = new FilterGroup();

            filterGroup.Filters.Add(ResolveFilter<T>(searchingFilter));
            pageRequest.FilterGroups.Add(filterGroup);
        }

        private static Filter ResolveFilter<T>(JqGridRequestSearchingFilter searchingFilter) where T : class
        {
            if (searchingFilter == null) throw new ArgumentNullException("searchingFilter");

            Type type = typeof(T);

            PropertyInfo prop = type.GetProperty(searchingFilter.SearchingName);
            if (prop == null) throw new ArgumentNullException("prop");

            Type valueType = prop.PropertyType;
            string dbFieldName = searchingFilter.SearchingName;

            var attribute =
                prop.GetCustomAttributes(typeof(SearchDataAttribute), false).FirstOrDefault() as SearchDataAttribute;
            if (attribute != null)
            {
                if (attribute.Type != null) valueType = attribute.Type;
                if (!string.IsNullOrWhiteSpace(attribute.DbName)) dbFieldName = attribute.DbName;
            }

            object filterValue = searchingFilter.SearchingValue;

            if (!valueType.IsAssignableFrom(typeof(string)))
            {
                filterValue = TypeDescriptor.GetConverter(valueType).ConvertFromString(searchingFilter.SearchingValue);
            }

            return new Filter
            {
                Operator = MapToOperator(searchingFilter.SearchingOperator),
                Property = dbFieldName,
                Value = filterValue
            };
        }

        private static ComparisonOperator MapToOperator(this JqGridSearchOperators jqGridOperator)
        {
            switch (jqGridOperator)
            {
                case JqGridSearchOperators.Eq:
                    return ComparisonOperator.IsEqualTo;
                case JqGridSearchOperators.Bn:
                    return ComparisonOperator.DoesNotStartWith;
                case JqGridSearchOperators.Bw:
                    return ComparisonOperator.StartsWith;
                case JqGridSearchOperators.Cn:
                    return ComparisonOperator.Contains;
                case JqGridSearchOperators.En:
                    return ComparisonOperator.DoesNotEndWith;
                case JqGridSearchOperators.Ew:
                    return ComparisonOperator.EndsWith;
                case JqGridSearchOperators.Ge:
                    return ComparisonOperator.IsGreaterThanOrEqualTo;
                case JqGridSearchOperators.Gt:
                    return ComparisonOperator.IsGreaterThan;
                case JqGridSearchOperators.Le:
                    return ComparisonOperator.IsLessThanOrEqualTo;
                case JqGridSearchOperators.Lt:
                    return ComparisonOperator.IsLessThan;
                case JqGridSearchOperators.Nc:
                    return ComparisonOperator.DoesNotContain;
                case JqGridSearchOperators.Ne:
                    return ComparisonOperator.IsNotEqualTo;
                case JqGridSearchOperators.Nn:
                    return ComparisonOperator.IsNotNull;
                case JqGridSearchOperators.Nu:
                    return ComparisonOperator.IsNull;
                default:
                    return ComparisonOperator.IsEqualTo;
                //case JqGridSearchOperators.EqualOrNotEqual: not supported
                //case JqGridSearchOperators.In: not supported
                //case JqGridSearchOperators.Ni: not supported
                //case JqGridSearchOperators.NoTextOperators: not supported
                //case JqGridSearchOperators.NullOperators: not supported
                //case JqGridSearchOperators.TextOperators: not supported
            }
        }

        private static JqGridResponse CreateResponse<T>(PageResponse<T> pageResponse) where T : class
        {
            var response = new JqGridResponse
            {
                PageIndex = pageResponse.StartIndex - 1,
                TotalRecordsCount = pageResponse.Total,
                TotalPagesCount =
                    pageResponse.PageSize == 0
                        ? 0
                        : (int) Math.Ceiling((double) pageResponse.Total/(double) pageResponse.PageSize)
            };
            return response;
        }

        public static JqGridJsonResult ToJqGridJsonResult<T, TModel>(this PageResponse<T> pageResponse)
            where T : class
            where TModel : JqGridRow
        {
            return pageResponse.ToJqGridJsonResult(Mapper.Map<T, TModel>);
        }


        private static JqGridJsonResult ToJqGridJsonResult<T, TModel>(this PageResponse<T> pageResponse, Func<T, TModel> convertMethod)
            where T : class
            where TModel : JqGridRow
        {

            var response = CreateResponse(pageResponse);

            if (convertMethod == null)
                throw new ArgumentNullException("convertMethod");

            IEnumerable<JqGridRecord> convertedData = pageResponse.Items.Select(x =>
            {
                var row = convertMethod(x);
                return new JqGridRecord(row.Id, row);
            });

            response.Records.AddRange(convertedData);


            return new JqGridJsonResult { Data = response };
        }

        public static JqGridOptions<T> GridOptions<T>(this HtmlHelper helper, string id, int? height = null, 
            int? width = null, int rowsPerPage = 20, List<int> columnsOrder = null, bool useDynamicScrolling = false) where T : class
        {
            var options = new JqGridOptions<T>(id)
            {
                Pager = true,
                Height = height ?? 360,
                Width = width,
                RowsNumber = rowsPerPage,
                ParametersNames = new JqGridParametersNames { PagesCount = "npage" },
                AutoEncode = true,
                ViewRecords = true,
                DataType = JqGridDataTypes.Json,
                MethodType = JqGridMethodTypes.Post,
                ShrinkToFit = true,
                AutoWidth = width == null,
                ColumnsRemaping = columnsOrder,
                TopPager = true,
                RowAttributes = "$.gridRowAttributes",
                MultiSort = true,
				OnInitGrid = "$.OnInitGrid"
            };

            if (useDynamicScrolling)
            {
                options.DynamicScrollingMode = JqGridDynamicScrollingModes.HoldVisibleRows;
            }
            else
            {
                options.DynamicScrollingMode = JqGridDynamicScrollingModes.Disabled;
                options.RowsList = new List<int> { 5, 10, 20, 50, 100 };
            }

            return options;
        }

        public static JqGridOptions<T> SetOptions<T>(this JqGridOptions<T> options, Action<JqGridOptions<T>> action)
        {
            action(options);
            return options;
        }

        public static JqGridHelper<T> Grid<T>(this HtmlHelper helper, JqGridOptions<T> options) where T : class
        {
            return new JqGridHelper<T>(options);
        }

        public static JqGridHelper<T> BuildTree<T>(this JqGridOptions<T> options)
        {
            options.RowsNumber = -1;
            options.TreeGridEnabled = true;
            options.TreeGridModel = JqGridTreeGridModels.Adjacency;
            options.Pager = false;
            options.TopPager = false;
            options.DynamicScrollingMode = JqGridDynamicScrollingModes.Disabled;

            return new JqGridHelper<T>(options);
        }

        public static JqGridHelper<T> BuildGrid<T>(this JqGridOptions<T> options)
        {
            return new JqGridHelper<T>(options);
        }

        public static JqGridHelper<T> BuildGrid<T>(this JqGridOptions<T> options, string readActionUrl,
            string addAction = null,
            string editAction = null, string deleteAction = null, string unDeleteAction = null,
            string historyAction = null, string exportAction = null,
            string clearFiltersFunc = null,
			bool columnChooser = true,
            bool showFilterToolbar = true) where T : class
        {
            options.Url = readActionUrl;
            var grid = new JqGridHelper<T>(options);

            var navigatorOptions = new JqGridNavigatorOptions {Search = true};
            var searchActionOptions = new JqGridNavigatorSearchActionOptions
            {
                AdvancedSearching = true,
                AdvancedSearchingWithGroups = false,
                CloneSearchRowOnAdd = false
            };

            var addActionOptions = !string.IsNullOrWhiteSpace(addAction)
                ? new JqGridNavigatorEditActionOptions {Url = addAction, CloseAfterEdit = true, CloseOnEscape = true}
                : null;
            var editActionOptions = !string.IsNullOrWhiteSpace(editAction)
                ? new JqGridNavigatorEditActionOptions {Url = editAction, CloseAfterEdit = true, CloseOnEscape = true}
                : null;
            var deleteActionOptions = !string.IsNullOrWhiteSpace(deleteAction)
                ? new JqGridNavigatorDeleteActionOptions {Url = deleteAction, CloseOnEscape = true}
                : null;

            grid.Navigator(navigatorOptions, editActionOptions, addActionOptions, deleteActionOptions,
                searchActionOptions);

	        if (columnChooser)
	        {
	            grid.AddNavigatorButton(new JqGridNavigatorButtonOptions
	            {
	                Caption = MUI.ColumnsChoserTitle,
	                OnClick = "$.columnChooser",
	            });
	        }

            if (showFilterToolbar)
            {
                grid = grid.FilterToolbar(new JqGridFilterToolbarOptions
                {
                    StringResult = true,
                    DefaultSearchOperator = JqGridSearchOperators.Eq,
                    GroupingOperator = JqGridSearchGroupingOperators.And,
                    SearchOperators = true
                });

                grid.AddNavigatorButton(new JqGridNavigatorButtonOptions
                {
                    Caption = MUI.ClearFilterButton,
                    OnClick = clearFiltersFunc ?? "$.clearFilters"
                });
            }
            grid.AddNavigatorSeparator();

            if (!string.IsNullOrWhiteSpace(historyAction))
            {
                grid.AddNavigatorButton(new JqGridNavigatorButtonOptions
                {
                    Caption = MUI.HistoryBtnTitle,
                    OnClick = string.Format(@"function(e){{$.showHistory.call(this, '{0}');}}", historyAction)
                });
                grid.AddNavigatorSeparator();
            }

            if (!string.IsNullOrWhiteSpace(unDeleteAction))
            {
                grid.AddNavigatorButton(new JqGridNavigatorButtonOptions
                {
                    Caption = MUI.UnDelete,
                    OnClick = string.Format(@"function(e){{$.unDeleteEntity.call(this, '{0}');}}", unDeleteAction)
                });
                grid.AddNavigatorSeparator();
            }

            if (!string.IsNullOrWhiteSpace(exportAction))
            {
                grid.AddNavigatorButton(new JqGridNavigatorButtonOptions
                {
                    Caption = "Export",
                    OnClick = string.Format(@"function(e){{$.exportData.call(this, '{0}');}}", exportAction)
                });
                grid.AddNavigatorSeparator();
            }

            return grid;
        }

        public static void ApplyCustomVisibilityRules(this List<JqGridColumnModel> colModels,
            IJqGridColumnCustomVisibilityRules visibilityRules)
        {
            visibilityRules.Apply(colModels);
        }
    }
}