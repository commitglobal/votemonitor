namespace Vote.Monitor.Api.Feature.PollingStation.GetTagValues;
public class Endpoint : Endpoint<Request, Results<Ok<List<TagModel>>, ProblemDetails>>
{
    private readonly VoteMonitorContext _context;

    public Endpoint(VoteMonitorContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/tags/values");
    }

    public override async Task<Results<Ok<List<TagModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var filter = req.Filter ?? new();

        var result = await _context
               .PollingStations
               .Where(x => x.ElectionRoundId == req.ElectionRoundId)
               .Where(station => filter.Count == 0 || EF.Functions.JsonContains(station.Tags, filter))
               .Where(station => EF.Functions.JsonExists(station.Tags, req.SelectTag))
               .Select(station => new TagModel
               {
                   Name = req.SelectTag,
                   Value = station.Tags.RootElement.GetProperty(req.SelectTag).GetString()!
               })
               .Distinct()
               .ToListAsync(cancellationToken: ct);

        return TypedResults.Ok(result);
    }
}
