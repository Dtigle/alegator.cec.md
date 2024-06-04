using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.Web.SRV.Resources;
using NHibernate.Util;

namespace CEC.Web.SRV.Models.Election
{
    public class UpdateElectionModel
    {
        public long Id { get; set; }

        [Display(Name = "UpdateElectionModel_ElectionType", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "UpdateElectionModel_ElectionTypeRequired", ErrorMessageResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public long ElectionType { get; set; }

        [Display(Name = "UpdateElectionModel_ElectionDate", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "UpdateElectionModel_ElectionDateRequired", ErrorMessageResourceType = typeof(MUI))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [DataType(DataType.Date)]
        [AdditionalMetadata("data-input-mask", "99.99.9999")]
        public DateTime? ElectionDate { get; set; }

        [Display(Name = "UpdateElectionModel_SaiseId", ResourceType = typeof(MUI))]
        public long SaiseId { get; set; }

        [Display(Name = "UpdateElectionModel_Comments", ResourceType = typeof(MUI))]
        [UIHint("MultilineText")]
        public string Comments { get; set; }

		[Display(Name = "UpdateElectionModel_AcceptAbroadDeclaration", ResourceType = typeof(MUI))]
		[UIHint("Checkbox")]
		public bool? AcceptAbroadDeclaration { get; set; }
    }
}