using Amdaris;
using CEC.SAISE.BLL;
using CEC.SAISE.BLL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.WebServices
{    

    public class MethodWeb
    {  

        public SearchResult SearshWebResults(string idnp)
        {
            var _votingBll = DependencyResolver.Current.Resolve<IVotingBll>();
            var shearsh = _votingBll.SearchVoterAsyncForWebService(idnp);
            var s = shearsh;
            return s;
        }
    }
}