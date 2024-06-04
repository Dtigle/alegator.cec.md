using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class VoterConflictModel
    {
        public long Id { get; set; }

        [Display(Name = "Person_Idnp", ResourceType = typeof(MUI))]
        public string Idnp { get; set; }
        [Display(Name = "Person_FirstName", ResourceType = typeof(MUI))]
        public string FirstName { get; set; }
        [Display(Name = "Person_Surname", ResourceType = typeof(MUI))]
        public string Surname { get; set; }
        [Display(Name = "Person_MiddleName", ResourceType = typeof(MUI))]
        public string MiddleName { get; set; }
        [Display(Name = "Person_DataOfBirth", ResourceType = typeof(MUI))]
        public string DateOfBirth { get; set; }
        [Display(Name = "Person_Gender", ResourceType = typeof(MUI))]
        public string Gender { get; set; }
        [Display(Name = "Person_DocumentSeries", ResourceType = typeof(MUI))]
        public string DocSeria { get; set; }
        [Display(Name = "Person_DocumentNumber", ResourceType = typeof(MUI))]
        public string DocNumber { get; set; }
        [Display(Name = "Person_DocumentType", ResourceType = typeof(MUI))]
        public string DocType { get; set; }
        [Display(Name = "Person_DocIssuedDate", ResourceType = typeof(MUI))]
        public string DocIssueDate { get; set; }
        [Display(Name = "Person_DocIssuedBy", ResourceType = typeof(MUI))]
        public string DocIssueBy { get; set; }
        [Display(Name = "Person_DocValidBy", ResourceType = typeof(MUI))]
        public string DocValidBy { get; set; }
        [Display(Name = "Person_Status", ResourceType = typeof(MUI))]
        public string PersonStatus { get; set; }
        [Display(Name = "Person_Address", ResourceType = typeof(MUI))]
        public string Address { get; set; }
        [Display(Name = "Person_Comments", ResourceType = typeof(MUI))]
        public string Comments { get; set; }
    }
}