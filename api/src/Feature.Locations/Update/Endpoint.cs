using Feature.Locations.Specifications;
using Vote.Monitor.Core.Helpers;

namespace Feature.Locations.Update;

public class Endpoint(IRepository<LocationAggregate> repository,
    IRepository<ElectionRoundAggregate> electionRoundRepository)
    : Endpoint<Request, Results<NoContent, NotFound<ProblemDetails>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/locations/{id}");
        DontAutoTag();
        Options(x => x.WithTags("locations"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound<ProblemDetails>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "Election round not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var location = await repository.SingleOrDefaultAsync(new GetLocationByIdSpecification(req.ElectionRoundId, req.Id), ct);
        if (location is null)
        {
            AddError(r => r.Id, "Location not found.");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        location.UpdateDetails(req.Level1, req.Level2, req.Level3, req.Level4, req.Level5, req.DisplayOrder, req.Tags.ToTagsObject());
        await repository.UpdateAsync(location, ct);
        electionRound.UpdateLocationsVersion();
        await electionRoundRepository.UpdateAsync(electionRound, ct);

        return TypedResults.NoContent();
    }
}
