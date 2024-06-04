using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.PollingStation
{
    public  class ElectionNumberListOrderBy
    {

        public ElectionNumberListOrderBy()
        {
            SelectedAPSIds = new List<long>();
        }

        public List<long> SelectedAPSIds { get; set; }

        [Required]
        [UIHint("Select2")]
        [Select2RemoteConfig("","GetDateForOrder","PollingStation","json", "electionListDataRequest", "electionListResults", pageLimit:10)]
        public long first { get; set; }
        
      
        
    }
   
   
}