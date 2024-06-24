
using Amdaris.NHibernateProvider;
using CEC.SAISE.BLL;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.App_Start;
using CEC.SAISE.EDayModule.WebServices.Modeldto;


namespace CEC.SAISE.EDayModule.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GetDataVoters" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select GetDataVoters.svc or GetDataVoters.svc.cs at the Solution Explorer and start debugging.
    public class GetDataVoters : IGetDataVoters
    {
        public ResultVotersdto GetVoter(string idnp)
        {
            ResultVotersdto result = new ResultVotersdto();
            try
            {
              //  var repositorry = IoC.Resolve<ISaiseRepository>();
                var _votingBll = IoC.Resolve<IVotingBll>();
                using (new NhUnitOfWork())
                {                   
                    var shearsh = _votingBll.SearchVoterAsyncForWebService(idnp);
                    var s = shearsh;
                    result.VoterCertificatatNumber = s.VoterData.NrCertificat;
                    result.VoterNumberList = s.VoterData.ElectionListNr?.ToString();
                    result.CircumscriptionName = s.VoterData.CircuscriptionName;
                    result.PollingStationNumber = s.VoterData.PolingStationNumber;
                    result.CircumscriptionNumber = s.VoterData.CircuscriptionNumber;

                }
            }
            catch (System.Exception e)
            {

                throw;
            }
                
           
               
             return result;
            
        }
    }
}
