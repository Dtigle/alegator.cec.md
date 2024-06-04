using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.Domain
{
    public class StayStatement : SRVBaseEntity, INotificationEntity
    {
        private readonly Person _person;
        private readonly PersonAddress _baseAddress;
        private readonly PersonAddress _declaredStayAddress;
        private readonly Election _electionInstance;

        public StayStatement()
        {
        }

        public StayStatement(Person person, PersonAddress baseAddress, PersonAddress declaredStayAddress,
            Election electionInstance)
        {
            _person = person;
            _baseAddress = baseAddress;
            _declaredStayAddress = declaredStayAddress;
            _electionInstance = electionInstance;
        }

        public virtual Person Person
        {
            get { return _person; }
        }

        public virtual PersonAddress BaseAddress 
        {
            get { return _baseAddress; }
        }

        public virtual PersonAddress DeclaredStayAddress
        {
            get { return _declaredStayAddress; }
        }

        public virtual Election ElectionInstance
        {
            get { return _electionInstance; }
        }

        string INotificationEntity.GetNotificationType()
        {
            return "StayStatement_Notification";
        }
    }
}
