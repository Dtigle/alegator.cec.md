namespace CEC.SAISE.Domain
{
    public enum AssignedVoterStatus
    {
        OtherList = -1,
        Undefined = 0,
        NoChange = 1,
        Unallocated = 2,

        BaseList = 1000,

        TransferRequest = 3000,

        ReceivedBallot = 5000,
        ReceivedBallotAbsentee = 5001,
        ReceivedBallotSupplementary = 5002,
        ReceivedBallotMobile = 5004,
        ReceivedBallotEmbassy = 5008,
        ReceivedBallotInternet = 5010,
        ReceivedBallotSupplementaryNew = 5020,

        #region CR1.8
        // teoretic aceste statute nu trebuie sa apara in AssignedVoters,
        // dar practica arata si asa situatii.
        // Statuturi luate din VoterStatus.
        Invalid = 9000,
        InvalidRejected = 9001,
        InvalidNotCitizen = 9002,
        InvalidCriminal = 9004,
        InvalidOutOfCountry = 9008,
        InvalidDied = 9010,
        InvalidDuplicateCheck = 9020,
        InvalidDuplicate = 9040,

        CaptureBusy = 9999,

        #endregion

    }
}