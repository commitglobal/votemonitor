using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.Entities.CoalitionAggregate;

public class Coalition : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public string Name { get; private set; }

    public Guid LeaderId { get; private set; }
    public MonitoringNgo Leader { get; private set; }
    public virtual List<CoalitionMembership> Memberships { get; internal set; } = [];
    public virtual List<CoalitionFormAccess> FormAccess { get; internal set; } = [];

    internal Coalition(ElectionRound electionRound, string name, MonitoringNgo leader,
        IEnumerable<MonitoringNgo> members)
    {
        Id = Guid.NewGuid();
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Name = name;
        Leader = leader;
        LeaderId = leader.Id;
        Memberships = members
            .Select(member=>CoalitionMembership.Create(electionRound, this, member))
            .DistinctBy(x => x.MonitoringNgoId).ToList();
    }

    public static Coalition Create(
        ElectionRound electionRound,
        string name,
        MonitoringNgo leaderNgo,
        IEnumerable<MonitoringNgo> members) => new(electionRound, name, leaderNgo, members);

    public void Update(string coalitionName, IEnumerable<CoalitionMembership> memberships)
    {
        var leader = Memberships.FirstOrDefault(x => x.MonitoringNgoId == LeaderId) ??
                     CoalitionMembership.Create(ElectionRoundId, Id, LeaderId);
        Memberships = memberships.Union([leader]).DistinctBy(x => x.MonitoringNgoId).ToList();
        Name = coalitionName;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private Coalition()
    {
    }
#pragma warning restore CS8618

}
