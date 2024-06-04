namespace CEC.SAISE.Domain
{
    public enum RoleLevel
    {
        Administrator = 0,
        CecAdministrator = 10,
        CecItStaff = 20,
        CecUser = 30,
        DataCentreCoordinator = 40,
        DataCentreSupervisors = 50,
        User = 70,
        LowestLevel = 100,
    }
}