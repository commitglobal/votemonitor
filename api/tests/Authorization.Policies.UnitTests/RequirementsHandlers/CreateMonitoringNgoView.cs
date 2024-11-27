namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class CreateMonitoringNgoView
{
    public static CreateMonitoringNgoView With() => new();

    internal MonitoringNgoView ArchivedElectionRound()
    {
        return new MonitoringNgoView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Archived,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Activated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Active
        };
    }
    internal MonitoringNgoView DeactivatedNgo()
    {
        return new MonitoringNgoView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Started,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Deactivated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Active
        };
    }
    internal MonitoringNgoView SuspendedMonitoringNgo()
    {
        return new MonitoringNgoView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Started,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Activated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Suspended
        };
    } 
    
    internal static MonitoringNgoView ForValidAccess()
    {
        return new MonitoringNgoView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Started,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Activated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Active
        };
    }

    private CreateMonitoringNgoView()
    {

    }
}
