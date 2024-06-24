using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Razor.Editor;
using CEC.SRV.Domain;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Address
{
    public class AddressAdministrativeGridModel : AddressGridModel
    {
		[Display(Name = "GeoLocation", ResourceType = typeof(MUI))]
		[JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
		[JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
		//[JqGridColumnEditable(false, EditType = JqGridColumnEditTypes.CheckBox, Value = "true:false")]
		[JqGridColumnSearchable(false, /*typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select,*/ SearchOperators = JqGridSearchOperators.NullOperators)]
        //[SearchData(DbName = "Address.geolatitude")]
		public bool GeoLocation { get; set; }
    }
}