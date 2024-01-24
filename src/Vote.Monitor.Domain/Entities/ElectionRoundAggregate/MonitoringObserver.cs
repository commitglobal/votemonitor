namespace Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

public class MonitoringObserver : ValueObject
{
    public int Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid ObserverId { get; private set; }
    public Observer Observer { get; private set; }
    public Guid InviterNgoId { get; private set; }
    public CSO InviterNgo { get; private set; }

#pragma warning disable CS8618 // Required by Entity Framework
    private MonitoringObserver()
    {

    }
#pragma warning restore CS8618

    public MonitoringObserver(Guid electionRoundId, Guid inviterNgoId, Guid observerId)
    {
        ElectionRoundId = electionRoundId;
        InviterNgoId = inviterNgoId;
        ObserverId = observerId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ElectionRoundId;
        yield return ObserverId;
    }
}
