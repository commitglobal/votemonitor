using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Vote.Monitor.Domain.Entities.CoalitionAggregate;

public class CoalitionGuideAccess
{
    public Guid CoalitionId { get; set; }
    public Coalition Coalition { get; set; }

    public Guid MonitoringNgoId { get; set; }
    public MonitoringNgo MonitoringNgo { get; set; }

    public Guid GuideId { get; set; }
    public ObserverGuide Guide { get; set; }

    public static CoalitionGuideAccess Create(Guid coalitionId, Guid monitoringNgoId, Guid guideId) => new()
    {
        CoalitionId = coalitionId, MonitoringNgoId = monitoringNgoId, GuideId = guideId
    };

    public static CoalitionGuideAccess Create(Coalition coalition,
        MonitoringNgo monitoringNgo,
        ObserverGuide guide) => new()
    {
        Guide = guide,
        GuideId = guide.Id,
        Coalition = coalition,
        CoalitionId = coalition.Id,
        MonitoringNgo = monitoringNgo,
        MonitoringNgoId = monitoringNgo.Id
    };

#pragma warning disable CS8618 // Required by Entity Framework
    private CoalitionGuideAccess()
    {
    }
#pragma warning restore CS8618
}
