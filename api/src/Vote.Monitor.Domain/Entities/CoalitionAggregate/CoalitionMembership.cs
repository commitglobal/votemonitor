using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.Entities.CoalitionAggregate;

public class CoalitionMembership
{
    public Guid ElectionRoundId { get; set; }
    public ElectionRound ElectionRound { get; set; }

    public Guid CoalitionId { get; set; }
    public Coalition Coalition { get; set; }

    public Guid MonitoringNgoId { get; set; }
    public MonitoringNgo MonitoringNgo { get; set; }

    public static CoalitionMembership Create(Guid electionRoundId, Guid coalitionId, Guid monitoringNgoId) => new()
    {
        ElectionRoundId = electionRoundId, CoalitionId = coalitionId, MonitoringNgoId = monitoringNgoId
    };

    public static CoalitionMembership Create(ElectionRound electionRound,
        Coalition coalition,
        MonitoringNgo monitoringNgo) => new()
    {
        ElectionRound = electionRound,
        Coalition = coalition,
        MonitoringNgo = monitoringNgo,
        ElectionRoundId = electionRound.Id,
        CoalitionId = coalition.Id,
        MonitoringNgoId = monitoringNgo.Id
    };

#pragma warning disable CS8618 // Required by Entity Framework
    private CoalitionMembership()
    {
    }
#pragma warning restore CS8618
}
