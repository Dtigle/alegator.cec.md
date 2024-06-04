using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class PeopleConflictModel
    {
        public long Id { get; set; }

        [Display(Name = "Conflict_Idnp", ResourceType = typeof(MUI))]
        public virtual string Idnp { get; set; }
        [Display(Name = "Conflict_Surname", ResourceType = typeof(MUI))]
        public virtual string LastName { get; set; }
        [Display(Name = "Conflict_FirstName", ResourceType = typeof(MUI))]
        public virtual string FirstName { get; set; }
        [Display(Name = "Conflict_MiddleName", ResourceType = typeof(MUI))]
        public virtual string SecondName { get; set; }
        [Display(Name = "Conflict_DataOfBirth", ResourceType = typeof(MUI))]
        public virtual string Birthdate { get; set; }
        [Display(Name = "Conflict_Gender", ResourceType = typeof(MUI))]
        public virtual string Gender { get; set; }
        [Display(Name = "Conflict_DocType", ResourceType = typeof(MUI))]
        public virtual string DocType { get; set; }
        [Display(Name = "Conflict_DocSeria", ResourceType = typeof(MUI))]
        public virtual string Series { get; set; }
        [Display(Name = "Conflict_DocNumber", ResourceType = typeof(MUI))]
        public virtual string Number { get; set; }
        [Display(Name = "Conflict_IssueDate", ResourceType = typeof(MUI))]
        public virtual string IssueDate { get; set; }
        [Display(Name = "Conflict_ExpirationDate", ResourceType = typeof(MUI))]
        public virtual string ExpirationDate { get; set; }
        [Display(Name = "Conflict_Validity", ResourceType = typeof(MUI))]
        public virtual string Validity { get; set; }
        [Display(Name = "Conflict_PersonStatus", ResourceType = typeof(MUI))]
        public virtual string PersonStatus { get; set; }
        [Display(Name = "Conflict_Address", ResourceType = typeof(MUI))]
        public string Address { get; set; }
        [Display(Name = "Conflict_Region", ResourceType = typeof(MUI))]
        public string Region { get; set; }
        [Display(Name = "Conflict_Locality", ResourceType = typeof(MUI))]
        public string Locality { get; set; }
        [Display(Name = "Conflict_AdministrativeCode", ResourceType = typeof(MUI))]
        public int AdministrativeCode { get; set; }
        [Display(Name = "Conflict_Source", ResourceType = typeof(MUI))]
        public virtual string Source { get; set; }
        [Display(Name = "Conflict_Status", ResourceType = typeof(MUI))]
        public virtual string Status { get; set; }
        [Display(Name = "Conflict_StatusMessage", ResourceType = typeof(MUI))]
        public virtual string StatusMessage { get; set; }
        [Display(Name = "Conflict_DataCreated", ResourceType = typeof(MUI))]
        public virtual string Created { get; set; }
        [Display(Name = "Conflict_StatusDate", ResourceType = typeof(MUI))]
        public virtual string StatusDate { get; set; }
        [Display(Name = "Conflict_Comments", ResourceType = typeof(MUI))]
        public virtual string Comments { get; set; }
    }
}