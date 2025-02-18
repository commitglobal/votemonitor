using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;

namespace Feature.Locations.Create;

public class Endpoint(
    VoteMonitorContext context,
    IRepository<ElectionRoundAggregate> electionRoundRepository,
    ITimeProvider timeProvider,
    ICurrentUserProvider userProvider)
    : Endpoint<Request, Results<Ok<Response>, NotFound<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("api/election-rounds/{electionRoundId}/locations");
        DontAutoTag();
        Options(x => x.WithTags("locations"));
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
        
        var entities = req
            .Locations
            .Select(x => LocationAggregate.Create(electionRound,
                x.Level1,
                x.Level2,
                x.Level3,
                x.Level4,
                x.Level5,
                x.DisplayOrder,
                x.Tags.ToTagsObject(),
                timeProvider.UtcNow,
                userProvider.GetUserId()!.Value))
            .ToList();

        await context.BulkInsertAsync(entities, cancellationToken: ct);

        electionRound.UpdateLocationsVersion();

        await electionRoundRepository.UpdateAsync(electionRound, cancellationToken: ct);

        return TypedResults.Ok(new Response() { RowsImported = entities.Count, });
    }
}
