using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.CountryAggregate;

namespace Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

public class ElectionRound : AuditableBaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    private ElectionRound()
    {

    }
#pragma warning restore CS8618

    public string Name { get; private set; }
    public ElectionRoundStatus Status { get; private set; } = ElectionRoundStatus.NotStarted;
    public Guid CountryId { get; private set; }
    public Country Country{ get; private set; }

    public ElectionRound(Guid countryId, string name, ITimeService timeService) : base(Guid.NewGuid(), timeService)
    {
        Name = name;
        CountryId = countryId;
        Status = ElectionRoundStatus.NotStarted;
    }

    public void UpdateDetails(string name)
    {
        Name = name;
    }
    public void MarkAsNotStarted()
    {
        // todo: add invariants
        Status = ElectionRoundStatus.NotStarted;
    }

    public void MarkAsStarted()
    {
        // todo: add invariants
        Status = ElectionRoundStatus.Started;
    }

    public void MarkAsArchived()
    {
        // todo: add invariants
        Status = ElectionRoundStatus.Archived;
    }
}
