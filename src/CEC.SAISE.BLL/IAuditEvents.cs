using CEC.SAISE.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.BLL.Dto;

namespace CEC.SAISE.BLL
{
   public interface IAuditEvents
    {
            Task InsertEvents( string code, SystemUser user, string message, string ipMashine);

        List<AuditEvents> GetAuditEvents(string code, string type);
    }
}
