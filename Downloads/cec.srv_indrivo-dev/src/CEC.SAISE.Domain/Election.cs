using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain
{
    public class Election : SaiseBaseEntity
    {
        public virtual ElectionType Type { get; set; }

        public virtual int Status { get; set; }

        public virtual DateTime DateOfElection { get; set; }

        public virtual string Comments { get; set; }

        public virtual string ReportsPath { get; set; }

        public virtual string GetFullName()
        {
            return string.Format("{0:dd.MM.yyyy} - {1}", this.DateOfElection, this.Comments);
        }

        public virtual bool IsSubTypeOfLocalElection()
        {
            return (Type.Id & ElectionType.Local) == ElectionType.Local;
        }

        public virtual bool IsSubTypeOfMayorElection()
        {
            return ((Type.Id == ElectionType.Local_PrimarLocal) ||
                    (Type.Id == ElectionType.Local_PrimarGeneral));
        }
    }
}
