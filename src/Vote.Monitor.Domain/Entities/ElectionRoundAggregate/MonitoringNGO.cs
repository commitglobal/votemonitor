namespace Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

public class MonitoringNGO : ValueObject
{
    public int Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid NgoId { get; private set; }
    public CSO Ngo { get; private set; }

#pragma warning disable CS8618 // Required by Entity Framework
    private MonitoringNGO()
    {

    }
#pragma warning restore CS8618

    public MonitoringNGO(Guid electionRoundId, Guid ngoId)
    {
        ElectionRoundId = electionRoundId;
        NgoId = ngoId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ElectionRoundId;
        yield return NgoId;
    }
}
