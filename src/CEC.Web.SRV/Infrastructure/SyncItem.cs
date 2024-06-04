using System.Collections.Generic;

namespace CEC.Web.SRV.Infrastructure
{
    public class SyncItem
    {
        public long Id { get; set; }

        public string Type { get; set; }

        public EnumEntityOperations Operation { get; set; }

        public Dictionary<string, object> Model { get; set; }
    }

    public enum EnumEntityOperations
    {
        Create = 1,

        Update = 2,

        Delete = 3,

        UnDelete = 4,

        PersistentDelete = 5
    }
}