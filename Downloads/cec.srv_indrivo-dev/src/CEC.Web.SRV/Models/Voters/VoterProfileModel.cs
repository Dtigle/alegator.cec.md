using CEC.Web.SRV.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace CEC.Web.SRV.Models.Voters
{
    public class VoterProfileModel
    {
        public long PersonId { get; set; }

        [Display(Name = "Person_Idnp", ResourceType = typeof(MUI))]
        public string IDNP { get; set; }

        [Display(Name = "Person_FirstName", ResourceType = typeof(MUI))]
        public string FirstName { get; set; }

        [Display(Name = "Person_Surname", ResourceType = typeof(MUI))]
        public string SurName { get; set; }

        [Display(Name = "Person_MiddleName", ResourceType = typeof(MUI))]
        public string MiddleName { get; set; }

        [Display(Name = "Person_Gender", ResourceType = typeof(MUI))]
        [UIHint("Gender")]
        public string Sex { get; set; }

        [Display(Name = "Person_Status", ResourceType = typeof(MUI))]
        public string Status { get; set; }

        [Display(Name = "Person_Age", ResourceType = typeof(MUI))]
        public string Age { get; set; }

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

        [Display(Name = "Lookups_DataModified", ResourceType = typeof(MUI))]
        public string Modified { get; set; }

        [Display(Name = "Lookups_ModifiedById", ResourceType = typeof(MUI))]
        public string ModifiedBy { get; set; }

    }
}