using EFCore.BulkExtensions;

namespace Vote.Monitor.Feature.PollingStation.Import;
public class Endpoint : Endpoint<Request, Results<Ok<Response>, NotFound, ProblemDetails>>
{
    private readonly VoteMonitorContext _context;
    private readonly IPollingStationParser _parser;

    public Endpoint(VoteMonitorContext context, IPollingStationParser parser)
    {
        _context = context;
        _parser = parser;
    }

    public override void Configure()
    {
        Post("/api/polling-stations/import");
        AllowFileUploads();
    }

    public override async Task<Results<Ok<Response>, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var parsingResult = await _parser.ParseAsync(req.File.OpenReadStream(), ct);
        if (parsingResult is PollingStationParsingResult.Fail failedResult)
        {
            foreach (var validationFailure in failedResult.ValidationErrors.SelectMany(x => x.Errors))
            {
                AddError(validationFailure);
            }

            ThrowIfAnyErrors();
        }

        var successResult = parsingResult as PollingStationParsingResult.Success;

        var entities = successResult!
            .PollingStations
            .Select(x => new PollingStationAggregate(x.Address, x.DisplayOrder, x.Tags.ToTagsObject()))
            .ToList();

        await _context.PollingStations.BatchDeleteAsync(cancellationToken: ct);
        await _context.BulkInsertAsync(entities, cancellationToken: ct);
        await _context.BulkSaveChangesAsync(cancellationToken: ct);

        return TypedResults.Ok(new Response { RowsImported = entities.Count });
    }
}
