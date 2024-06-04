using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SRV.BLL.Extensions;
using Microsoft.AspNet.Identity;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Impl
{
   public  class VoterCertificatBll :IVoterCertificatBll
    {
        private readonly ISaiseRepository _repository;

        private readonly IAuditEvents _auditEvents;
        public VoterCertificatBll(ISaiseRepository repository , IAuditEvents auditEvents)
        {
            _repository = repository;
            _auditEvents = auditEvents;
        }

        public PageResponse<VoterCertificatDto> GetVoterCertificat(PageRequest pageRequest )
        {
            var currentPrincipal = SecurityHelper.GetLoggedUser();
            var user =  _repository.GetAsync<SystemUser>(currentPrincipal.Identity.GetUserId<long>());

            
            VoterCertificatDto certificat = null;
            ElectionDay election = null;
            AssignedVoter assigned = null;
            Voter voter = null;
            PollingStation polingstation = null;
            VoterCertificat voterCertificat = null;
            var electionDay = _repository.Query<ElectionDay>().FirstOrDefault();

            var certificats = _repository.QueryOver(()=>voterCertificat)
                .JoinAlias(()=> voterCertificat.AssignedVoter,()=> assigned)
                .JoinAlias(()=> assigned.Voter,()=> voter)
                .JoinAlias(()=> voterCertificat.PollingStation,()=>polingstation)
                .Where(x=>x.PollingStation.Id==user.Result.PollingStationId&&x.DeletedDate==null);

           
             var s= certificats.SelectList(list => list            
             .Select(x => x.Id).WithAlias(() => certificat.Id)
            .Select(x=> voter.Idnp).WithAlias(()=>certificat.Idnp)
            .Select(x => voter.LastNameRo).WithAlias(() => certificat.LastName)
            .Select(x => voter.NameRo).WithAlias(() => certificat.FirstName)           
            .Select(x => x.ReleaseDate).WithAlias(() => certificat.ReleaseDate)
            .Select(x => x.CertificatNr).WithAlias(() => certificat.CertificatNr)
            .Select(x => polingstation.NameRo).WithAlias(() => certificat.PollingStation)
            .Select(x => polingstation.Id).WithAlias(() => certificat.PollingStationId)
            
            )
            .TransformUsing(Transformers.AliasToBean<VoterCertificatDto>())
            .RootCriteria.CreatePage<VoterCertificatDto>(pageRequest);
            foreach (var sItem in s.Items)
            {
                if (electionDay != null) sItem.Election = electionDay.ElectionDayDate.Date;
            }

            
            return s;
        }

        public PageResponse<AssignedVoter> GetVoter(PageRequest pageRequest, string idnp)
        {
            var voter = _repository.Query<Voter>().FirstOrDefault(x => x.Idnp == Convert.ToInt64(idnp));
            var voterQuery = _repository.QueryOver<AssignedVoter>().Fetch(x => x.VoterCertificats).Lazy
                .Where(x =>x.Voter==voter).RootCriteria.CreatePage<AssignedVoter>(pageRequest);
            return voterQuery;


        }





        public  VoterCertificat GetForCreateCertificat(long id )

        {
            var assignedVoter = _repository.Query<AssignedVoter>().FirstOrDefault(x => x.Id == id);
            
            var voter = new VoterCertificat
            {
                AssignedVoter = assignedVoter,
                
            };

            return voter;
        }

        public async Task<bool> SaveCertificatAsync(long id,long assignedVoterId, string CertificatNumber, long PllingStationId , DateTime? releaseDate)
        {
            
            var currentPrincipal = SecurityHelper.GetLoggedUser();
            var user = await _repository.GetAsync<SystemUser>(currentPrincipal.Identity.GetUserId<long>());
           
            var assignedVoter = _repository.Query<AssignedVoter>().FirstOrDefault(x =>x.Id== assignedVoterId);
            var pollingStation = _repository.Get<PollingStation>(PllingStationId);
            var idCert = id; 

            try
            {
                if (idCert == 0)
                {
                    var voterCertificat = new VoterCertificat { AssignedVoter = assignedVoter,  PollingStation = pollingStation, CertificatNr = CertificatNumber, EditUser = user, ReleaseDate = releaseDate,  EditDate = DateTime.Now  };
                    _repository.SaveOrUpdate(voterCertificat);
                  
                    return true;
                }
                else
                {
                    var certificat = _repository.Get<VoterCertificat>(idCert);
                    certificat.ReleaseDate = releaseDate;
                    certificat.CertificatNr = CertificatNumber;                    
                    _repository.SaveOrUpdate(certificat);
                
                    return true;
                }
               
            }
            catch(Exception e)
            {
                return false;
            }
            
        }

        public  VoterCertificatDto GetCertificat(long id)
        {    
            var cert = _repository.Get<VoterCertificat>(id);
            var election = _repository.Query<ElectionDay>().FirstOrDefault();

            
                var cartificat = new VoterCertificatDto
                {
                    Id = cert.Id,
                    PollingStation = cert.PollingStation.Region.Name,
                    Adress = cert.AssignedVoter.Voter.Region.RegionType.Name+ cert.AssignedVoter.Voter.Region.Name + cert.AssignedVoter.Voter.GetAddress(),
                    BirthDate = cert.AssignedVoter.Voter.DateOfBirth,
                    CertificatNr = cert.CertificatNr,
                    DocumentData = cert.AssignedVoter.Voter.DateOfIssue,
                    DocumentExpireData = cert.AssignedVoter.Voter.DateOfExpiry,
                    DocumentNr = cert.AssignedVoter.Voter.DocumentNumber,
                    Election = election.ElectionDayDate.DateTime,
                    ElectionType = election.Name,
                    FirstName = cert.AssignedVoter.Voter.NameRo,
                    LastName = cert.AssignedVoter.Voter.LastNameRo,
                    Idnp = cert.AssignedVoter.Voter.Idnp,
                    ReleaseDate = cert.ReleaseDate,
                    PollingStationRegion =cert.PollingStation.NameRo

                };
            
                return cartificat;
            
        }

        public async Task<bool> DeleteCerificat(long id)
        {
           

            var certificat = _repository.Get<VoterCertificat>(id);
            if(certificat != null)
            {
                certificat.DeletedDate = DateTime.Now;
               
                    _repository.SaveOrUpdate(certificat);
                
              
               
                return true;
            }
            else
            {
                return false;
            }

           
        }

        public async Task<ElectionDay> GetDateOfElection()
        {
            var election = _repository.Query<ElectionDay>().FirstOrDefault();
            return election;
        }




    }
}
