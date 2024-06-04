using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Impl
{
    public class ElectionResultsBll : IElectionResultsBll
    {
        private readonly ISaiseRepository _saiseRepository;

        public ElectionResultsBll(ISaiseRepository saiseRepository)
        {
            _saiseRepository = saiseRepository;
        }

        public async Task<BallotPaperDto> GetBallotPaperAsync(long electionRoundId, long pollingStationId)
        {
            var ballotPaper = await _saiseRepository.QueryAsync<BallotPaper>(z =>
                z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionRoundId && x.PollingStation.Id == pollingStationId));
            var assignedPollingStation = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                z.FirstOrDefault(x => x.ElectionRound.Election.Id == electionRoundId && x.PollingStation.Id == pollingStationId));

            if (ballotPaper == null)
            {
                return null;
            }

            return MapBallotPaper(ballotPaper, assignedPollingStation);
        }


        public async Task<BallotPaperDto> GetBallotPaperAsync(long ballotPaperId)
        {
            var ballotPaper = await _saiseRepository.QueryAsync<BallotPaper>(z =>
                z.FirstOrDefault(x => x.Id == ballotPaperId));

            if (ballotPaper == null)
            {
                return null;
            }

            var assignedPollingStation = await _saiseRepository.QueryAsync<AssignedPollingStation>(z =>
                z.FirstOrDefault(x => x.ElectionRound.Id == ballotPaper.ElectionRound.Id && x.PollingStation.Id == ballotPaper.PollingStation.Id));

            if (assignedPollingStation == null)
            {
                return null;
            }

            return MapBallotPaper(ballotPaper, assignedPollingStation);
        }

        public async Task<SaveUpdateResult> SaveUpdateResults(BallotPaperDataDto ballotPaperDto, BallotPaperStatus bpStatusToBeSet)
        {
            var ballotPaper = await _saiseRepository.QueryAsync<BallotPaper>(z =>
                z.FirstOrDefault(x => x.Id == ballotPaperDto.BallotPaperId));

            var validationResult = ValidateBallotPaper(ballotPaper, ballotPaperDto, bpStatusToBeSet);

            if (validationResult != BallotPaperValidationStatus.IsValid)
            {
                return new SaveUpdateResult { Success = false, ValidationStatus = validationResult };
            }

            var userIsAdmin = SecurityHelper.LoggedUserIsInRole("Administrator");

            if (ballotPaper.IsResultsConfirmed && !userIsAdmin)
            {
                return new SaveUpdateResult { Success = false, ValidationStatus = BallotPaperValidationStatus.MultipleConfirmationsProhibited };
            }

            TransferData(ballotPaperDto, ballotPaper, bpStatusToBeSet);

            await _saiseRepository.SaveOrUpdateAsync(ballotPaper);

            return new SaveUpdateResult { Success = true, ValidationStatus = validationResult };
        }

        private void TransferData(BallotPaperDataDto ballotPaperDto, BallotPaper ballotPaper, BallotPaperStatus bpStatusToBeSet)
        {
            var userId = SecurityHelper.GetLoggedUserId();
            var userProxy = _saiseRepository.LoadProxy<SystemUser>(userId);

            ballotPaper.RegisteredVoters = ballotPaperDto.RegisteredVoters;
            ballotPaper.Supplementary = ballotPaperDto.Supplementary;
            ballotPaper.BallotsIssued = ballotPaperDto.BallotsIssued;
            ballotPaper.BallotsCasted = ballotPaperDto.BallotsCasted;
            ballotPaper.DifferenceIssuedCasted = ballotPaperDto.DifferenceIssuedCasted;
            ballotPaper.BallotsValidVotes = ballotPaperDto.BallotsValidVotes;
            ballotPaper.BallotsReceived = ballotPaperDto.BallotsReceived;
            ballotPaper.BallotsUnusedSpoiled = ballotPaperDto.BallotsUnusedSpoiled;
            ballotPaper.BallotsSpoiled = ballotPaperDto.BallotsSpoiled;
            ballotPaper.BallotsUnused = ballotPaperDto.BallotsUnused;

            ballotPaper.Status = bpStatusToBeSet;

            ballotPaper.EditUser = userProxy;

            foreach (var competitorResult in ballotPaperDto.CompetitorResults)
            {
                var electionResult = ballotPaper.ElectionResults.First(x => x.Id == competitorResult.ElectionResultId);
                if (electionResult.PoliticalParty.Status != PoliticalPartyStatus.Rejected)
                {
                    electionResult.BallotCount = competitorResult.BallotCount;
                    electionResult.Status = (bpStatusToBeSet == BallotPaperStatus.Approved)
                        ? ElectionResultStatus.Approved
                        : ElectionResultStatus.WaitingForApproval;

                    electionResult.EditUser = userProxy;
                    electionResult.EditDate = DateTime.Now;
                }
            }

            if (bpStatusToBeSet == BallotPaperStatus.WaitingForApproval)
            {
                ballotPaper.EditDate = DateTime.Now;
            }

            if (bpStatusToBeSet == BallotPaperStatus.Approved)
            {
                ballotPaper.IsResultsConfirmed = true;
                ballotPaper.ConfirmationUserId = userId;
                ballotPaper.ConfirmationDate = DateTime.Now;
            }
        }

        private BallotPaperValidationStatus ValidateBallotPaper(BallotPaper ballotPaper, BallotPaperDataDto ballotPaperDto, BallotPaperStatus bpStatusToBeSet)
        {
            if (ballotPaper == null)
            {
                return BallotPaperValidationStatus.NotFound;
            }

            //if (bpStatusToBeSet == BallotPaperStatus.WaitingForApproval && ballotPaper.Status != BallotPaperStatus.New)
            //{
            //    return BallotPaperValidationStatus.InvalidStatus;
            //}

            if (bpStatusToBeSet == BallotPaperStatus.Approved &&
                ballotPaper.Status != BallotPaperStatus.WaitingForApproval)
            {
                return BallotPaperValidationStatus.InvalidStatus;
            }

            return BallotPaperValidationStatus.IsValid;
        }

        private BallotPaperDto MapBallotPaper(BallotPaper ballotPaper, AssignedPollingStation assignedPollingStation)
        {
            try
            {

                var result = new BallotPaperDto
                {
                    BallotPaperId = ballotPaper.Id,
                    RegisteredVoters = ballotPaper.RegisteredVoters,
                    Supplementary = ballotPaper.Supplementary,
                    BallotsIssued = ballotPaper.BallotsIssued,
                    BallotsCasted = ballotPaper.BallotsCasted,
                    DifferenceIssuedCasted = ballotPaper.DifferenceIssuedCasted,
                    BallotsValidVotes = ballotPaper.BallotsValidVotes,
                    BallotsReceived = ballotPaper.BallotsReceived,
                    BallotsUnusedSpoiled = ballotPaper.BallotsUnusedSpoiled,
                    BallotsSpoiled = ballotPaper.BallotsSpoiled,
                    BallotsUnused = ballotPaper.BallotsUnused,
                    EditDate = ballotPaper.EditDate,
                    EditUser = ballotPaper.EditUser.UserName,
                    IsResultsConfirmed = ballotPaper.IsResultsConfirmed,
                    ConfirmationUserId = ballotPaper.ConfirmationUserId,
                    ConfirmationDate = ballotPaper.ConfirmationDate,
                    Status = ballotPaper.Status,
                    ElectionType = ballotPaper.ElectionRound.Election.Type.ElectionCompetitorType,
                    OpeningVotersCount = assignedPollingStation.OpeningVoters,
                    CompetitorResults = ballotPaper.ElectionResults
                  .Where(x => (x.PoliticalParty.AssignedCircumscription != null && x.PoliticalParty.AssignedCircumscription.Id == assignedPollingStation.AssignedCircumscription.Id) || (x.PoliticalParty.AssignedCircumscription == null))
                  .OrderBy(x => x.BallotOrder)
                  .Select(x => MapElectionResult(x, assignedPollingStation))
                  .ToList(),



                };
                SetPermissionsForCurrentUser(result, assignedPollingStation.ImplementsEVR);
                return result;

            }
            catch (Exception e)
            {
                return null;

            }



        }

        private void SetPermissionsForCurrentUser(BallotPaperDto result, bool apsImplementsEVR)
        {
            var principal = SecurityHelper.GetLoggedUser();
            var isAdmin = principal.IsInRole("Administrator");
            var isPollingOfficer = principal.Identity.HasPermission(SaisePermissions.ResultsEdit);
            if (result.Status == BallotPaperStatus.New)
            {
                result.AllowSubmitResults = ((!(isPollingOfficer ^ apsImplementsEVR)) || isAdmin);
            }
            else
            {
                if (!principal.Identity.HasPermission(SaisePermissions.AllowElectionResultsVerification))
                {
                    return;
                }

                result.AllowSubmitResults = ((!(isPollingOfficer ^ apsImplementsEVR)) || isAdmin) &&
                                            (result.Status != BallotPaperStatus.Approved);
                if (principal.Identity.HasPermission(SaisePermissions.AllowElectionResultsVerification))
                {
                    result.AllowSubmitConfirmation = (result.Status != BallotPaperStatus.Approved) && (isAdmin);
                }
            }
        }

        private CompetitorResultDto MapElectionResult(ElectionResult electionResult, AssignedPollingStation assignedPollingStation)
        {

            var result = new CompetitorResultDto
            {
                ElectionResultId = electionResult.Id,
                BallotOrder = electionResult.BallotOrder,
                BallotCount = electionResult.BallotCount,
                PartyStatus = GetPartyStatus(electionResult.PoliticalParty, assignedPollingStation)
            };

            SetCompetitorName(electionResult, ref result);

            return result;
        }

        private PoliticalPartyStatus GetPartyStatus(ElectionCompetitor politicalParty, AssignedPollingStation assignedPollingStation)
        {
            var result = politicalParty.Status;
            var election = assignedPollingStation.ElectionRound.Election;
            var electionOverrides = politicalParty.StatusOverrides.Where(x => x.ElectionRound.Election.Id == election.Id).ToList();
            PoliticalPartyStatusOverride overridenStatus = null;

            if (electionOverrides.Count > 0)
            {
                if (election.IsSubTypeOfLocalElection())
                {
                    if ((election.Type.Id == ElectionType.Local_ConsilieriLocal) ||
                        (election.Type.Id == ElectionType.Local_PrimarLocal))
                    {
                        overridenStatus = electionOverrides.FirstOrDefault(
                                x => x.AssignedCircumscription.Id == assignedPollingStation.AssignedCircumscription.Id);
                    }
                    else
                    {
                        overridenStatus = electionOverrides.FirstOrDefault(
                                x => x.AssignedCircumscription.Id == assignedPollingStation.AssignedCircumscription.Id /*&& x.AssignedCircumscription.Region.Id == -1*/);
                    }
                }
                else
                {
                    overridenStatus = electionOverrides.FirstOrDefault(x => x.AssignedCircumscription.Id == -1 /*&& x.AssignedCircumscription.Region.Id == -1*/);
                }
            }

            return overridenStatus != null ? overridenStatus.Status : result;
        }

        private void SetCompetitorName(ElectionResult electionResult, ref CompetitorResultDto result)
        {
            var candidateName = "";

            if (electionResult.Candidate != null && electionResult.Candidate.Id > 0)
            {
                candidateName = string.Format("{0} {1}", electionResult.Candidate.LastNameRo,
                    electionResult.Candidate.NameRo);
            }

            result.IsIndependent = electionResult.PoliticalParty.IsIndependent;
            result.PoliticalPartyCode = electionResult.PoliticalParty.Code;

            result.PoliticalPartyName = electionResult.PoliticalParty.NameRo;
            result.CandidateName = candidateName;
        }

        public void TransferEDayData(string serverIpAddress, string remoteUserName, string remotePassword)
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["AmdarisConnectionString"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[MoveDataToRepository]", cn))
                {
                    cmd.CommandTimeout = 30000;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@serverName", SqlDbType.VarChar)
                    {
                        Value = "SAISE.RepLink",
                        Direction = ParameterDirection.Input
                    });
                    cmd.Parameters.Add(new SqlParameter("@serverIpAddress", SqlDbType.VarChar)
                    {
                        Value = serverIpAddress,
                        Direction = ParameterDirection.Input
                    });
                    cmd.Parameters.Add(new SqlParameter("@localUsername", SqlDbType.VarChar)
                    {
                        Value = "sa",
                        Direction = ParameterDirection.Input
                    });
                    cmd.Parameters.Add(new SqlParameter("@remoteUsername", SqlDbType.VarChar)
                    {
                        Value = remoteUserName,
                        Direction = ParameterDirection.Input
                    });
                    cmd.Parameters.Add(new SqlParameter("@remotePassword", SqlDbType.VarChar)
                    {
                        Value = remotePassword,
                        Direction = ParameterDirection.Input
                    });

                    SqlParameter execStatus = new SqlParameter("@execStatus", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(execStatus);

                    SqlParameter execMsg = new SqlParameter("@execMsg", SqlDbType.VarChar)
                    {
                        Size = 5000,
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(execMsg);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }


        public List<DataTransferStage> GetDataTransferStages()
        {
            if (CheckLinkedServerExists())
            {
                ISession session = ((SaiseRepository)_saiseRepository).SessionFactory.GetCurrentSession();
                IQuery query = session.CreateSQLQuery(@"exec GetDataTransferStages @serverName=:serverName, @execStatus=:execStatus OUTPUT, @execMsg=:execMsg OUTPUT");
                query.SetParameter("serverName", "SAISE.RepLink", NHibernateUtil.String);
                query.SetParameter("execStatus", "", NHibernateUtil.String);
                query.SetParameter("execMsg", "", NHibernateUtil.String);
                var result = query.SetResultTransformer(Transformers.AliasToBean<DataTransferStage>()).List<DataTransferStage>();
                return result.ToList();
            }
            return null;
        }


        public bool CheckLinkedServerExists()
        {
            ISession session = ((SaiseRepository)_saiseRepository).SessionFactory.GetCurrentSession();
            IQuery query = session.CreateSQLQuery(@"
    SET NOCOUNT ON;
    DECLARE @retval int = 0,
            @sysservername sysname;
    BEGIN TRY
        SELECT  @sysservername = CONVERT(sysname, :serverName);
        EXEC @retval = sys.sp_testlinkedserver @sysservername;
        SELECT 1;
    END TRY
    BEGIN CATCH
        SELECT 0;
    END CATCH;");
            query.SetParameter("serverName", "SAISE.RepLink", NHibernateUtil.String);
            var result = query.UniqueResult();
            return result.ToString() == "0" ? false : true;
        }

        public bool AproveBallotPaper(List<long> model)
        {
            try
            {
                var userId = SecurityHelper.GetLoggedUserId();
                const string baseQry = @"
               update BallotPaper set Status = :status , ConfirmationUserId = :UserId  , IsResultsConfirmed=:Confirmation, ConfirmationDate = :date   where Status=1 and BallotPaperId IN(:bpList); ";

                var queryBuilder = _saiseRepository.CreateSqlStringBuilder(baseQry, null)
                    .SetParameter("status", 2)
                    .SetParameter("UserId", userId)
                    .SetParameter("Confirmation", true)
                    .SetParameter("date", DateTime.Now)
                    .SetParameterList("bpList", model);
                queryBuilder.ToSqlQuery().ExecuteUpdate();

                return true;
            }
            catch (Exception e)
            {

                return false;
            }

        }
    }
}
