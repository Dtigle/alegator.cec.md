using CEC.SRV.BLL.Dto;

namespace CEC.Web.SRV.Models.Election
{
    public class ElectionRequest : Select2Request
    {
        public bool? AcceptStayDeclaration { get; set; }
    }
}