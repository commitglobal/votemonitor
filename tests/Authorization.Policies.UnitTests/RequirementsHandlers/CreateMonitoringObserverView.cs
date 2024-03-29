using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Authorization.Policies.UnitTests.RequirementsHandlers;

public class CreateMonitoringObserverView
{
    public static CreateMonitoringObserverView With() => new();

    internal MonitoringObserverView ArchivedElectionRound()
    {
        return new MonitoringObserverView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Archived,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Activated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Active,
            ObserverId = Guid.NewGuid(),
            UserStatus = UserStatus.Active,
            MonitoringObserverId = Guid.NewGuid(),
            MonitoringObserverStatus = MonitoringObserverStatus.Active
        };
    }
    internal MonitoringObserverView DeactivatedNgo()
    {
        return new MonitoringObserverView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Started,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Deactivated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Active,
            ObserverId = Guid.NewGuid(),
            UserStatus = UserStatus.Active,
            MonitoringObserverId = Guid.NewGuid(),
            MonitoringObserverStatus = MonitoringObserverStatus.Active
        };
    }

    internal MonitoringObserverView SuspendedMonitoringNgo()
    {
        return new MonitoringObserverView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Started,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Activated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Suspended,
            ObserverId = Guid.NewGuid(),
            UserStatus = UserStatus.Active,
            MonitoringObserverId = Guid.NewGuid(),
            MonitoringObserverStatus = MonitoringObserverStatus.Active
        };
    } 
    
    internal MonitoringObserverView DeactivatedObserver()
    {
        return new MonitoringObserverView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Started,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Activated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Active,
            ObserverId = Guid.NewGuid(),
            UserStatus = UserStatus.Deactivated,
            MonitoringObserverId = Guid.NewGuid(),
            MonitoringObserverStatus = MonitoringObserverStatus.Active
        };
    } 
    internal MonitoringObserverView SuspendedMonitoringObserver()
    {
        return new MonitoringObserverView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Started,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Activated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Active,
            ObserverId = Guid.NewGuid(),
            UserStatus = UserStatus.Active,
            MonitoringObserverId = Guid.NewGuid(),
            MonitoringObserverStatus = MonitoringObserverStatus.Suspended
        };
    } 
    
    internal static MonitoringObserverView ForValidAccess()
    {
        return new MonitoringObserverView
        {
            ElectionRoundId = Guid.NewGuid(),
            ElectionRoundStatus = ElectionRoundStatus.Started,
            NgoId = Guid.NewGuid(),
            NgoStatus = NgoStatus.Activated,
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringNgoStatus = MonitoringNgoStatus.Active,
            ObserverId = Guid.NewGuid(),
            UserStatus = UserStatus.Active,
            MonitoringObserverId = Guid.NewGuid(),
            MonitoringObserverStatus = MonitoringObserverStatus.Active
        };
    }

    private CreateMonitoringObserverView()
    {

    }
}
