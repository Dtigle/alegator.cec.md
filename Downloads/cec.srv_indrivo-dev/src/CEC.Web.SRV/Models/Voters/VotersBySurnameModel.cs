using CEC.Web.SRV.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Controllers;

namespace CEC.Web.SRV.Models.Voters
{
    public class VotersBySurnameModel
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
        
        public VoterAdressModel BaseAdress { get; set; }
    }
}