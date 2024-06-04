using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.Print;
using CEC.Web.SRV.Resources;
using NHibernate.Linq;

namespace CEC.SRV.BLL.Impl
{
    public class ElectionBll : Bll, IElectionBll
    {
        public ElectionBll(ISRVRepository repository) : base(repository)
        {
        }

        //public void SaveOrUpdate(long electionId, long electionTypeId, DateTime electionDate, long saiseId, string comments, bool acceptAbroadDeclaration)
        //      {
        //          var electionType = Repository.LoadProxy<ElectionType>(electionTypeId);
        //          var election = electionId == 0 ? new Election() : Repository.Get<Election>(electionId);

        //	if (acceptAbroadDeclaration && !election.AcceptAbroadDeclaration)
        //	{
        //		RemoveOtherAcceptAbroadDeclarations();
        //	}

        //          election.ElectionType = electionType;
        //          election.ElectionDate = electionDate;
        //          election.SaiseId = saiseId;
        //          election.Comments = comments;
        //	election.AcceptAbroadDeclaration = acceptAbroadDeclaration;

        //          Repository.SaveOrUpdate(election);
        //      }


        public void SaveOrUpdate(long? id, DateTime electionDate, string nameRo, string nameRu,
    string description, long electionTypeId,
    ElectionStatus status, string statusReason, string reportsPath)
        {
            var entity = id == null ? new Election() : Get<Election>(id.Value);

            entity.NameRo = nameRo;
            entity.NameRu = nameRu;
            entity.Description = description;

            if (id == null)
            {
                entity.ElectionType = Repository.LoadProxy<ElectionType>(electionTypeId);
            }

            if (entity.Status != status)
            {
                entity.Status = status;
                entity.StatusDate = DateTimeOffset.Now;
                entity.StatusReason = statusReason;
            }

            entity.ReportsPath = reportsPath;


            SaveOrUpdate(entity);

            if (!id.HasValue)
            {

                SaveOrUpdateElectionRound(
                    null,
                    entity.Id,
                    1,
                    null,
                    electionDate,
                    null,
                    null,
                    null,
                    ElectionRoundStatus.New
                    );
            }

        }


        public void SaveOrUpdateElectionRound(long? id, long electionId, int number, string description,
    DateTime electionDate, DateTime? campaignStartDate, DateTime? campaignEndDate, string reportsPath, ElectionRoundStatus status)
        {
            var entity = id == null ? new ElectionRound() : Get<ElectionRound>(id.Value);
            entity.Election = Repository.LoadProxy<Election>(electionId);
            entity.Number = number;
            entity.NameRo = "Tur " + number;
            entity.NameRu = null;
            entity.Description = description;
            entity.ElectionDate = electionDate;
            entity.CampaignStartDate = campaignStartDate;
            entity.CampaignEndDate = campaignEndDate;
            entity.ReportsPath = reportsPath;
            entity.Status = status;
            SaveOrUpdate(entity);
        }

        public IList<Election> GetActiveElections()
        {
            return Repository.Query<Election>().Where(x => x.Deleted == null).ToList();
        }

        public IList<ElectionRound> GetElectionRounds(long electionId)
        {
            return Repository.Query<Election>().Fetch(s => s.ElectionRounds).FirstOrDefault(x => x.Id == electionId).ElectionRounds.ToList();
        }

        public Election GetCurrentElection()
        {
            return Repository.Query<Election>().Where(x => x.Deleted == null && x.ElectionType.AcceptAbroadDeclaration == true).FirstOrDefault();
        }

        private void RemoveOtherAcceptAbroadDeclarations()
        {
            Repository.Query<Election>()
                .Where(x => x.ElectionType.AcceptAbroadDeclaration)
                .ForEach(x =>
                {
                    x.ElectionType.AcceptAbroadDeclaration = false;
                    Repository.SaveOrUpdate(x);
                });
        }

        public void VerificationIfElectionHasReference(long electionDayId)
        {
            var saiseExporter = Repository.Query<SaiseExporter>().FirstOrDefault(x => x.ElectionDayId == electionDayId && x.Deleted == null);
            if (saiseExporter != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                saiseExporter.ElectionDayId, saiseExporter.GetObjectType()));
            }

            var printSession = Repository.Query<PrintSession>().FirstOrDefault(x => x.Election.Id == electionDayId && x.Deleted == null);
            if (printSession != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                printSession.Election.GetObjectType(), printSession.GetObjectType()));
            }

            var stayStatement = Repository.Query<StayStatement>().FirstOrDefault(x => x.ElectionInstance.Id == electionDayId && x.Deleted == null);
            if (stayStatement != null)
            {
                throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
                stayStatement.ElectionInstance.GetObjectType(), stayStatement.GetObjectType()));
            }
        }
    }
}
