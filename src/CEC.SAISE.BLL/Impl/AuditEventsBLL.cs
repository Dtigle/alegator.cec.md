using Amdaris.Domain;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.Web.SRV.Infrastructure;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SAISE.BLL.Dto;

namespace CEC.SAISE.BLL.Impl
{
   public class AuditEventsBLL : IAuditEvents
   {
        private readonly ISaiseRepository _repository;
        public AuditEventsBLL(ISaiseRepository repository)
        {
            _repository = repository;
        }
        public async Task  InsertEvents( string code, SystemUser user , string message ,string ipMashine)
        {
            try
            {
                var typeEvent = _repository.Query<AuditEventTypes>().FirstOrDefault(x => x.code.Trim() == code.Trim());
                if (typeEvent != null)
                {
                    var events = new AuditEvents
                    {
                        level = 1,
                        EditDate = DateTime.Now,
                        userId = user.UserName,
                        AuditEventsTypes = typeEvent,
                        generatedAt = DateTime.Now,
                        EditUser = user,
                        message = message,
                        userMachineIp = ipMashine

                    };

                    await _repository.SaveOrUpdateAsync(events);
                    
                }
            }
            catch
            {
                //
            }
           
        }

       public  List<AuditEvents> GetAuditEvents(string code, string type)
       {
               return  _repository.Query<AuditEvents>().Where(x => x.message.Trim() == code.Trim() && x.AuditEventsTypes.code == type).ToList();
       }
    }
}
