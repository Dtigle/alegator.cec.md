using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto.Concurents
{
    public class PersonalDataResponse
    {
        public bool Success { get; set; }

        public string Idnp { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public DateTime DateOfBirth { get; set; }

        public GenderType Gender { get; set; }
    }
}
