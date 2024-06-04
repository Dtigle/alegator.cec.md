namespace CEC.SAISE.Domain
{
    public enum ElectionResultStatus
    {
        New = 0,
        WaitingForApproval = 10,
        Approved = 20,
        MissingInformation = 30,
        Rejected = 40,
    }
}