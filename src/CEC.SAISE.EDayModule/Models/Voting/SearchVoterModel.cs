using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SAISE.BLL.Dto;

namespace CEC.SAISE.EDayModule.Models.Voting
{
    public class SearchVoterModel
    {
        public string Idnp { get; set; }

        public SearchResult Result { get; set; }
    }
}