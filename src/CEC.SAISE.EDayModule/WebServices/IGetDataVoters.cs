using CEC.SAISE.EDayModule.WebServices.Modeldto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CEC.SAISE.EDayModule.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGetDataVoters" in both code and config file together.
    [ServiceContract]
    public interface IGetDataVoters
    {
        [OperationContract]
        ResultVotersdto GetVoter(string idnp);
    }
}
