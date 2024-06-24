using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CEC.SAISE.Domain
{
    public class Voter : SaiseBaseEntity
    {
        private readonly IList<AssignedVoter> _assignedVoters;     
     
       
       
        public Voter()
        {     

            _assignedVoters = new List<AssignedVoter>();

           
         
        }

        
        public virtual string NameRo { get; set; }

        
        public virtual string LastNameRo { get; set; }
      
        public virtual string PatronymicRo { get; set; }
       
        public virtual string NameRu { get; set; }

        public virtual string LastNameRu { get; set; }

        
        public virtual string PatronymicRu { get; set; }

        public virtual DateTime DateOfBirth { get; set; }

        public virtual string PlaceOfBirth { get; set; }

        public virtual string PlaceOfResidence { get; set; }

        public virtual GenderType Gender { get; set; }

        public virtual DateTime DateOfRegistration { get; set; }

        public virtual long Idnp { get; set; }
        public virtual long? ElectionListNumber { get; set; }
        public virtual string DocumentNumber { get; set; }

        public virtual DateTime? DateOfIssue { get; set; }

        public virtual DateTime? DateOfExpiry { get; set; }

        public virtual VoterStatus Status { get; set; }

        public virtual long? BatchId { get; set; }
        public virtual Region Region { get; set; }

        public virtual long? StreetId { get; set; }

        public virtual string StreetName { get; set; }

        public virtual long? StreetNumber { get; set; }

        public virtual string StreetSubNumber { get; set; }

        public virtual long? BlockNumber { get; set; }

        public virtual string BlockSubNumber { get; set; }

        public virtual IReadOnlyCollection<AssignedVoter> AssignedVoters
        {
            get { return new ReadOnlyCollection<AssignedVoter>(_assignedVoters); }
        }
       



        public virtual string GetAddress()
        {
            var streetName = GetStreetName();
            if (streetName == string.Empty)
            {
                return string.Empty;
            }

            var buildingPart = GetBuildingNumber();
            if (buildingPart == string.Empty)
            {
                return streetName;
            }
            //de modificat 
            return string.Format("{0}, {1}, {2}", streetName, buildingPart, GetAppartment());
        }

        private string GetStreetName()
        {
            return (string.IsNullOrEmpty(StreetName) || StreetName.StartsWith("$")) ? string.Empty : StreetName;
        }

        private string GetBuildingNumber()
        {
            return string.IsNullOrWhiteSpace(StreetSubNumber)
                ? StreetNumber.GetValueOrDefault() == 0 ? string.Empty : StreetNumber.ToString()
                : string.Format(char.IsDigit(StreetSubNumber.First()) ? "{0}/{1}" : "{0}{1}", StreetNumber,
                    StreetSubNumber);
        }

        private string GetAppartment()
        {
            return string.IsNullOrWhiteSpace(BlockSubNumber)
                ? BlockNumber.GetValueOrDefault() == 0? string.Empty : "ap."+BlockNumber.ToString()
                : string.Format("ap.{0}-{1}", BlockNumber, BlockSubNumber);
        }

        
    }
}
