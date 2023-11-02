namespace Vote.Monitor.Feature.PollingStation.GetTagValues;
public class Endpoint : Endpoint<Request, Results<Ok<List<TagModel>>, NotFound, ProblemDetails>>
{
    private readonly VoteMonitorContext _context;

    public Endpoint(VoteMonitorContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/tags/values");
        RequestBinder(new RequestBinder());
    }

    public override async Task<Results<Ok<List<TagModel>>, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var filter = req.Filter ?? new();

        var result = await _context
               .PollingStations
               .Where(station => filter.Count == 0 || EF.Functions.JsonContains(station.Tags, filter))
               .Where(station => EF.Functions.JsonExists(station.Tags, req.SelectTag))
               .Select(station => new TagModel
               {
                   Key = req.SelectTag,
                   Value = station.Tags.RootElement.GetProperty(req.SelectTag).GetString()!
               })
               .Distinct()
               .ToListAsync(cancellationToken: ct);

        if (result.Any())
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }
}
