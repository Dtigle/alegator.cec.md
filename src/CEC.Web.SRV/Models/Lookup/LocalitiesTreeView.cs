
using System.Web.Mvc;
using CEC.Web.SRV.Models.Grid;

namespace CEC.Web.SRV.Models.Lookup
{
    public class LocalitiesTreeView
	{
		public string Name { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string DataCreated { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string DataModified { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string DataDeleted { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string CreatedById { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ModifiedById { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string DeletedById { get; set; }
	}
}