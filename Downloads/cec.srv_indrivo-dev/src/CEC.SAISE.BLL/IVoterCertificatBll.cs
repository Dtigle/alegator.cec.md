using Amdaris.Domain.Paging;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL
{
   public interface IVoterCertificatBll 
    {

        PageResponse<VoterCertificatDto> GetVoterCertificat(PageRequest pageRequest);
        PageResponse<AssignedVoter> GetVoter(PageRequest pageRequest, string idnp);


        
        VoterCertificat GetForCreateCertificat(long idnp );
        VoterCertificatDto GetCertificat(long id);


         Task<bool> SaveCertificatAsync(long id,long assingmentID,  string CertificatNumber, long PllingStationId, DateTime? releaseDate);

        Task<bool> DeleteCerificat(long id);
        Task<ElectionDay> GetDateOfElection();


    }
}
