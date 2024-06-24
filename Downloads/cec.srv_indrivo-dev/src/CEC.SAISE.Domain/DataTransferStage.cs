using Amdaris.Domain;
using System;

namespace CEC.SAISE.Domain
{
    public class DataTransferStage : Entity
    {
        public virtual string TableName { get; set; }

        public virtual decimal Processed { get; set; }

        public virtual decimal Total { get; set; }

        public virtual decimal Percent
        {
            get
            {
                if (Total == 0)
                {
                    return 100;
                }
                else
                {
                    return Math.Round(Processed / Total * 100, 2);
                }
            }
        }
    }
}
