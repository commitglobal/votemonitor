using Vote.Monitor.Core.Entities;
using Vote.Monitor.Domain.Entities.CountryAggregate;

namespace Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

public class ElectionRound : BaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    private ElectionRound()
    {

    }

    public string Name { get; private set; }
    public ElectionRoundStatus Status { get; private set; } = ElectionRoundStatus.NotStarted;
    public Guid CountryId { get; set; }
    public Country Country{ get; set; }

    public ElectionRound(Guid countryId, string name)
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
