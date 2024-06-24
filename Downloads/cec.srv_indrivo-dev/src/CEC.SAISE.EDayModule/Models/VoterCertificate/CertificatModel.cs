using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amdaris.Domain;

namespace CEC.SAISE.EDayModule.Models.VoterCertificate
{
    public class CertificatModel : IEntity
    {
        public long Id { get; set; }
        public DateTime ElectionDate { get; set; }
        public virtual string IDNP { get; set; }

        public virtual string FullName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [DataType(DataType.Date)]
        [AdditionalMetadata("data-input-mask", "99.99.9999")]
        public virtual DateTime BirthDate { get; set; }


        public virtual string DocumentNumber { get; set; }


        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Required(ErrorMessage ="Data eliberării este obligatorie")]
        [CertificateDateValidation]
        public virtual DateTime? ReleaseDate { get; set; }

        [Required(ErrorMessage = "Nr. certificatului este obligatoriu")]
        public virtual string CertificatNr { get; set; }

        public virtual string Office { get; set; }

        

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [DataType(DataType.Date)]
        [AdditionalMetadata("data-input-mask", "99.99.9999")]
        public virtual DateTime? DocumentData { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [DataType(DataType.Date)]
        [AdditionalMetadata("data-input-mask", "99.99.9999")]
        public virtual DateTime? DocumentExpireData { get; set; }   
       

         public virtual string Adres { get; set; }

       
        public virtual string TypeElection { get; set; }
            
        public virtual string Circumscription { get; set; }
        public virtual string ElectionOffice { get; set; }

        public virtual int VoterSearchStatus { get; set; }

        public virtual long PolingStationId { get; set; }
        public virtual long AssignedVoterId { get; set; }
        public virtual long VoterId { get; set; }
        public virtual long ElectionId { get; set; }
    }
}