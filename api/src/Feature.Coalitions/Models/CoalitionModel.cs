using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Feature.NgoCoalitions.Models;

public class CoalitionModel
{
    public Guid Id { get; init; }
    public bool IsInCoalition { get; set; }
    public string Name { get; init; } = string.Empty;
    public Guid LeaderId { get; set; }
    public string LeaderName { get; set; } = string.Empty;

    public int NumberOfMembers => Members.Length;
    public CoalitionMember[] Members { get; init; } = [];

    public static CoalitionModel? FromEntity(Coalition? coalition)
    {
        return coalition is null
            ? null
            : new CoalitionModel
            {
                Id = coalition.Id,
                Name = coalition.Name,
                LeaderId = coalition.Leader.NgoId,
                LeaderName = coalition.Leader.Ngo.Name,
                IsInCoalition = true,
                Members = coalition.Memberships.Select(x => CoalitionMember.FromEntity(x.MonitoringNgo)).ToArray()
            };
    }
}
