using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto
{
    public class SearchResult
    {
        public VoterSearchStatus Status { get; set; }

        public VoterData VoterData { get; set; }
    }

    public enum VoterUtanStatus
    {
        Success,
        NotUtan,
        WrongUtan,
    }

    public enum VoterSearchStatus
    {
        None,
        Success,
        NotFound,
        FoundMultiple,
        NotAssigned
    }

    public class VoterData
    {
        public VoterData(Voter voter, AssignedVoter assignedVoter)
        {
            Idnp = voter.Idnp.ToString("D13");
            FirstName = voter.NameRo;
            LastName = voter.LastNameRo;
            Patronymic = voter.PatronymicRo;
            DateOfBirth = voter.DateOfBirth;
            DocumentNumber = voter.DocumentNumber;
            IsDocumentValid = (!voter.DateOfExpiry.HasValue || voter.DateOfExpiry >= DateTime.Now);
            Address =voter.Region.RegionType.Name+" "+voter.Region.Name+" , " +voter.GetAddress();
            VoterId = voter.Id;
            Gender = voter.Gender;
            VoterStatus = voter.Status;
            ElectionListNr = voter.ElectionListNumber;
            Certificat = assignedVoter!=null&&assignedVoter.VoterCertificats.FirstOrDefault(x => x.DeletedDate == null) != null ? true : false;
            NrCertificat = Certificat != false ? assignedVoter.VoterCertificats.FirstOrDefault(x => x.DeletedDate == null)?.CertificatNr : null;
            ReleaseDateCert = Certificat != false ? assignedVoter.VoterCertificats.FirstOrDefault(x => x.DeletedDate == null)?.ReleaseDate : null;
            if (assignedVoter != null)
            {
                Assignement = new VoterAssignementData(assignedVoter);
            }
        }


        public string Idnp { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        

        public DateTime? DateOfBirth { get; set; }

        public string DocumentNumber { get; set; }

        public bool IsDocumentValid { get; set; }
        
        public  string CompletPolingStationAdress { get; set; }
        public string PolingStationNumber { get; set; }
        public string CircuscriptionName { get; set; }
        public string CircuscriptionNumber { get; set; }
        public string Address { get; set; }

        public long VoterId { get; set; }
        public bool Certificat { get; set; }
        public string NrCertificat { get; set; }
        public long? ElectionListNr { get; set; }
        public DateTime? ReleaseDateCert { get; set; }
        public GenderType Gender { get; set; }

        public VoterAssignementData Assignement { get; set; }

        public VoterStatus VoterStatus { get; set; }

        public bool HasValidStatus
        {
            get { return VoterStatus < VoterStatus.Invalid; }
        }

        public bool HasVoted
        {
            get { return (Assignement != null) && (Assignement.VoteStatus >= AssignedVoterStatus.ReceivedBallot); }
        }

        public bool IsNotAssigned
        {
            get { return Assignement == null; }
        }

        public bool SetReceivedBallotAction { get; private set; }
        public bool SetReceivedAbsenteeBallotAction { get; private set; }
        public bool SetReceivedBallotMobileAction { get; private set; }
        public bool SetVoterAddSupplementaryAction { get; private set; }

        public void ValidateForUser(IIdentity identity, SystemUser user)
        {
            if (Assignement != null)
            {
                Assignement.ValidateForUser(identity, user);
            }
            SetActions(identity);
        }

        private static readonly AssignedVoterStatus[] potentialSupplimentaryStatuses =
        {
            AssignedVoterStatus.BaseList, AssignedVoterStatus.Undefined,
            AssignedVoterStatus.TransferRequest, AssignedVoterStatus.Unallocated,
            AssignedVoterStatus.OtherList
        };

        private void SetActions(IIdentity identity)
        {
            var allAssignementValid = Assignement != null && Assignement.AllAssignementValid();
            var isBaseListVoter = Assignement != null && Assignement.VoteStatus == AssignedVoterStatus.BaseList;
            SetReceivedBallotAction = HasValidStatus && allAssignementValid && isBaseListVoter && identity.HasPermission(SaisePermissions.ReceivedBallot);
            SetReceivedAbsenteeBallotAction = HasValidStatus && allAssignementValid && isBaseListVoter && identity.HasPermission(SaisePermissions.ReceivedAbsenteeBallot);
            SetReceivedBallotMobileAction = HasValidStatus && allAssignementValid && isBaseListVoter && identity.HasPermission(SaisePermissions.ReceivedBallotMobile);

            if (IsNotAssigned)
            {
                SetVoterAddSupplementaryAction = HasValidStatus && !allAssignementValid &&
                                                 identity.HasPermission(SaisePermissions.VoterAddSupplementary);
            }
            else
            {
                SetVoterAddSupplementaryAction = HasValidStatus && !allAssignementValid &&
                                 potentialSupplimentaryStatuses.Any(x => Assignement != null && x == Assignement.VoteStatus) &&
                                 identity.HasPermission(SaisePermissions.VoterAddSupplementary);
            }
        }

        public VoterData()
        {

        }
    }

    public class VoterAssignementData
    {
        public VoterAssignementData()
        {

        }

        public VoterAssignementData(AssignedVoter assignedVoter)
        {
            AssignedVoterId = assignedVoter.Id;
            Region = new ValueNamePair(assignedVoter.Region.Id, assignedVoter.Region.Name);
            PollingStation = new ValueNamePair(assignedVoter.PollingStation.Id,
            assignedVoter.PollingStation.GetFullName());
            PollingAdressLink = !string.IsNullOrWhiteSpace(assignedVoter.PollingStation.Region.GetFullPath())?"("+assignedVoter.PollingStation.Region.GetFullPath()+")": assignedVoter.PollingStation.Region.GetFullPath();
            VoteStatus = assignedVoter.Status;
            EditDate = assignedVoter.EditDate;
            EditUser = assignedVoter.EditUser.UserName;
        }

        public long AssignedVoterId { get; set; }

        public AssignedVoterStatus VoteStatus { get; set; }

        public string PollingAdressLink { get; set; }

        public ValueNamePair Region { get; set; }

        public ValueNamePair PollingStation { get; set; }

        public ValueNamePair Circumscription { get; set; }
        public DateTime EditDate { get; set; }

        public string ServerEditDate
        {
            get { return EditDate.ToString("dd.MM.yyyy HH:mm:ss"); }
        }


        public string EditUser { get; set; }
        public bool IsSameRegion { get; private set; }
        public bool IsSamePollingStation { get; private set; }
        public bool IsSameCircumscription { get; set; }
        public bool HideCircumscriptionLabel { get; set; }
        public bool HidePollingLabel { get; set; }

        public void ValidateForUser(IIdentity identity, SystemUser user)
        {
            IsSameRegion = true;// Region.Id == user.RegionId.GetValueOrDefault();
            IsSamePollingStation = PollingStation.Id == user.PollingStationId.GetValueOrDefault();
        }

        public bool AllAssignementValid()
        {
            return IsSameRegion && IsSamePollingStation;
        }
    }
}
