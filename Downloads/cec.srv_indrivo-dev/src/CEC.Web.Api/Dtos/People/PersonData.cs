using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CEC.Web.Api.Dtos.People
{
    [DataContract]
    public class PersonData
    {
        [DataMember]
        public string Idnp { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Patronymic { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public PersonGender Gender { get; set; }
    }
}