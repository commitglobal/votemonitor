using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.PollingStation.Create;

public class Endpoint(
    VoteMonitorContext context,
    IRepository<ElectionRoundAggregate> electionRoundRepository,
    ITimeProvider timeProvider,
    ICurrentUserProvider userProvider)
    : Endpoint<Request, Results<Ok<Response>, NotFound<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("api/election-rounds/{electionRoundId}/polling-stations");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "Election round not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var userId = userProvider.GetUserId()!.Value;

        var pollingStations = req.PollingStations.Select(ps => PollingStationAggregate.Create(electionRound,
                ps.Level1,
                ps.Level2,
                ps.Level3,
                ps.Level4,
                ps.Level5,
                ps.Number,
                ps.Address,
                ps.DisplayOrder,
                ps.Tags.ToTagsObject(),
                timeProvider.UtcNow,
                userId))
            .ToArray();

        await context.BulkInsertAsync(pollingStations, cancellationToken: ct);

        electionRound.UpdatePollingStationsVersion();

        await electionRoundRepository.UpdateAsync(electionRound, cancellationToken: ct);

        return TypedResults.Ok<Response>(new Response()
        {
            PollingStations = pollingStations.Select(x => new PollingStationModel
            {
                Id = x.Id,
                Level1 = x.Level1,
                Level2 = x.Level2,
                Level3 = x.Level3,
                Level4 = x.Level4,
                Level5 = x.Level5,
                Number = x.Number,
                Address = x.Address,
                DisplayOrder = x.DisplayOrder,
                Tags = x.Tags.ToDictionary(),
            }).ToArray()
        });
    }
}
