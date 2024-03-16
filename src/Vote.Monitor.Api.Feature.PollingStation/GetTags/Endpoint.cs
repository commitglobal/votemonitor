namespace Vote.Monitor.Api.Feature.PollingStation.GetTags;
public class Endpoint : Endpoint<Request, List<string>>
{
    private readonly VoteMonitorContext _context;

    public Endpoint(VoteMonitorContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/tags");
    }

    public override async Task<List<string>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var tags = await _context
              .PollingStations
              .Where(x=>x.ElectionRoundId == req.ElectionRoundId)
              .Select(x => Postgres.Functions.ObjectKeys(x.Tags))
              .Distinct()
              .ToListAsync(cancellationToken: ct);

        return tags;
    }
}
