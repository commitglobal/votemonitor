namespace Vote.Monitor.Feature.PollingStation.GetTags;
public class Endpoint : EndpointWithoutRequest<List<string>>
{
    private readonly VoteMonitorContext _context;

    public Endpoint(VoteMonitorContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/tags");
    }

    public override async Task<List<string>> ExecuteAsync(CancellationToken ct)
    {
        var tags = await _context
              .PollingStations
              .Select(x => Postgres.Functions.ObjectKeys(x.Tags))
              .Distinct()
              .ToListAsync(cancellationToken: ct);

        return tags;
    }
}
