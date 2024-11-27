using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.NgoCoalitions.Models;

public class CoalitionMember
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public static CoalitionMember FromEntity(MonitoringNgo ngo)
    {
        return new CoalitionMember { Id = ngo.NgoId, Name = ngo.Ngo.Name };
    }
}
