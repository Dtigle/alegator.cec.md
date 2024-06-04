
namespace CEC.Web.SRV.Models.Conflict
{
    public class ViewStatusConflictModel : StatusMessageConflictModel
    {
        public VoterConflictModel VoterData { get; set; }

        public PeopleConflictModel PeopleData { get; set; }
    }
}