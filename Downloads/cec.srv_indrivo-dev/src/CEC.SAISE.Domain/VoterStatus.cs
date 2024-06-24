namespace CEC.SAISE.Domain
{
    /// <summary>
    /// Format: SEEE
    ///     where SS - Status
    ///     and   EE - Error of Status (if any)
    /// </summary>
    public enum VoterStatus
    {
        //States
        Undefined = 0,
        NoChange = 1,

        Imported = 1000,
        ImportedError = 1001,
        ImportedDuplicateIdnp = 1002,

        BaseList = 4000,
        BaseListEdited = 4001,
        BaseListSupplementary = 4002,


        Invalid = 9000,
        InvalidRejected = 9001,
        InvalidNotCitizen = 9002,
        InvalidCriminal = 9004,
        InvalidOutOfCountry = 9008,
        InvalidDied = 9010,
        InvalidDuplicateCheck = 9020,
        InvalidDuplicate = 9040,

        CaptureBusy = 9999,
    }
}