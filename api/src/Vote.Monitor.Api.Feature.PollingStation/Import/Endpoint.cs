using Vote.Monitor.Api.Feature.PollingStation.Helpers;
using Vote.Monitor.Api.Feature.PollingStation.Services;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.PollingStation.Import;
public class Endpoint(
    IRepository<ElectionRoundAggregate> electionRoundRepository,
    VoteMonitorContext context,
    IPollingStationParser parser,
    ITimeProvider timeProvider,
    ICurrentUserIdProvider userProvider)
    : Endpoint<Request, Results<Ok<Response>, NotFound<ProblemDetails>, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations:import");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
        AllowFileUploads();
    }

    public override async Task<Results<Ok<Response>, NotFound<ProblemDetails>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "Election round not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var parsingResult = parser.Parse(req.File.OpenReadStream());
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
            .Select(x => PollingStationAggregate.Create(electionRound,
                x.Level1,
                x.Level2,
                x.Level3,
                x.Level4,
                x.Level5,
                x.Number,
                x.Address,
                x.DisplayOrder,
                x.Tags.ToTagsObject(),
                timeProvider.UtcNow,
                userProvider.GetUserId()!.Value))
            .ToList();

        await context.BulkInsertAsync(entities, cancellationToken: ct);

        electionRound.UpdatePollingStationsVersion();

        await electionRoundRepository.UpdateAsync(electionRound, cancellationToken: ct);

        return TypedResults.Ok(new Response { RowsImported = entities.Count });
    }
}
