
namespace CEC.SRV.BLL.Dto
{
    public class PeopleConflictDataDto
    {
        public virtual string Idnp { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string SecondName { get; set; }
        public virtual string Birthdate { get; set; }
        public virtual string SexCode { get; set; }
        public virtual string Dead { get; set; }
        public virtual string CitizenRm { get; set; }
        public virtual string IdentDocument { get; set; }
        public virtual string Registration { get; set; }
        public virtual string StatusConflictCode { get; set; }
        public virtual string Source { get; set; }
        public virtual string Status { get; private set; }
        public virtual string StatusMessage { get; set; }
        public virtual string Created { get; set; }
        public virtual string StatusDate { get; set; }
        public virtual string Comments { get; set; }
    }
}