namespace CEC.SRV.Domain
{
    public class Event : SRVBaseEntity
    {
        private readonly EventTypes _eventType;
        private readonly string _entityType;
        private readonly long _entityId;

        public Event()
        {
            
        }

        public Event(EventTypes eventType, string entityType, long entityId)
        {
            _eventType = eventType;
            _entityType = entityType;
            _entityId = entityId;
        }

        public virtual EventTypes EventType
        {
            get
            {
                return _eventType;
            }
        }

        public virtual string EntityType
        {
            get { return _entityType; }
        }

        public virtual long EntityId
        {
            get { return _entityId; }
        }
    }
}
