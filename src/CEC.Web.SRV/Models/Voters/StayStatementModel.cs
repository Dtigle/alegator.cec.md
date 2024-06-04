﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.SRV.Domain;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
    public class StayStatementModel
    {
        [Display(Name = "StayStatement_Id", ResourceType = typeof(MUI))]
        public long Id { get; set; }

        [Display(Name = "Personal_Data", ResourceType = typeof(MUI))]
        public PersonModel PersonInfo { get; set; }

        [Display(Name="AddressOfResidence", ResourceType = typeof(MUI))]
        public PersonAddressModel BaseAddressInfo { get; set; }

        [Display(Name = "DeclaredAddress", ResourceType = typeof(MUI))]
        public PersonAddressModel DeclaredStayAddressInfo { get; set; }

        [Display(Name = "ElectionInfo", ResourceType = typeof(MUI))]
        public ElectionModelStaytment ElectionInfo { get; set; }

		[Display(Name = "StayStatement_Region", ResourceType = typeof(MUI))]
		[UIHint("Select2")]
		[Required(ErrorMessageResourceName = "StayStatementErrorRequired_StayStatementRegionId", ErrorMessageResourceType = typeof(MUI))]
		[Select2RemoteConfig("", "GetRegions", "Voters", "json", "regionDataRequestStayStatement", "regionResultsStayStatement", "GetRegionName", "Voters", PageLimit = 10)]
		public long StayStatementRegionId { get; set; }

		[Display(Name = "StayStatement_PollingStation", ResourceType = typeof(MUI))]
		[UIHint("Select2")]
		[Required(ErrorMessageResourceName = "StayStatementErrorRequired_StayStatementPollingStationId", ErrorMessageResourceType = typeof(MUI))]
		[Select2RemoteConfig("", "GetPollingStations", "Voters", "json", "votersDataRequestStayStatement", "votersResultsStayStatement", "GetPollingStationName", "Voters", PageLimit = 10)]
		public long StayStatementPollingStationId { get; set; }

		public RegionStreetsType RegionStreetsType { get; set; }

		public bool HasStreets { get; set; }
    }
}