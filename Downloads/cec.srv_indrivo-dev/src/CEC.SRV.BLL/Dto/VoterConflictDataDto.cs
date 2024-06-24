using System;

namespace CEC.SRV.BLL.Dto
{
    public class VoterConflictDataDto
    {
        public long Id { get; set; }
        public string Idnp { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string DocSeria { get; set; }
        public string DocNumber { get; set; }
        public string DocType { get; set; }
        public DateTime? DocIssueDate { get; set; }
        public string DocIssueBy { get; set; }
        public DateTime? DocValidBy { get; set; }
        public string PersonStatus { get; set; }
        public string Address { get; set; }
        public string Comments { get; set; }
    }
}