using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SAISE.BLL.Dto;

namespace CEC.SAISE.EDayModule.Models
{
    public class ValueNameModel
    {
        public ValueNameModel()
        {
            
        }

        public ValueNameModel(ValueNamePair data)
        {
            Id = data.Id;
            Name = HttpUtility.JavaScriptStringEncode(data.Name);
        }

        public long Id { get; set; }

        public string Name { get; set; }
    }
}