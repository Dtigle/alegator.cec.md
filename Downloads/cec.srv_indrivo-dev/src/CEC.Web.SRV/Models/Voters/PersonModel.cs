using System;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
    public class PersonModel
    {
        public long PersonId { get; set; }

        [Display(Name = "Person_Idnp", ResourceType = typeof(MUI))]
        public string IDNP { get; set; }

        [Display(Name = "Person_FullName", ResourceType = typeof(MUI))]
        public string FullName { get; set; }

        [Display(Name = "Person_DataOfBirth", ResourceType = typeof(MUI))]
        [UIHint("Date")]
        public DateTime DateOfBirth { get; set; }

        public string DocType { get; set; }

        public string DocNumber { get; set; }

        [Display(Name = "Person_DocIssuedDate", ResourceType = typeof(MUI))]
        [UIHint("Date")]
        public DateTime? DocIssuedDate { get; set; }

        [Display(Name = "Person_DocIssuedBy", ResourceType = typeof(MUI))]
        public string DocIssuedBy { get; set; }

        [UIHint("Date")]
        [Display(Name = "Person_DocValidBy", ResourceType = typeof(MUI))]
        public DateTime? DocValidBy { get; set; }
    }
}