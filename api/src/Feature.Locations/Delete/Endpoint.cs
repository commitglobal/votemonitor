using Feature.Locations.Specifications;

namespace Feature.Locations.Delete;
public class Endpoint(IRepository<LocationAggregate> repository,
    IRepository<ElectionRoundAggregate> electionRoundRepository)
    : Endpoint<Request, Results<NoContent, NotFound<ProblemDetails>, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/locations/{id}");
        DontAutoTag();
        Options(x => x.WithTags("locations"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound<ProblemDetails>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "Election round not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var location = await repository.FirstOrDefaultAsync(new GetLocationByIdSpecification(req.ElectionRoundId, req.Id), ct);

        if (location is null)
        {
            AddError(r => r.Id, "Location not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        await repository.DeleteAsync(location, ct);

        electionRound.UpdateLocationsVersion();
        await electionRoundRepository.UpdateAsync(electionRound, ct);

        return TypedResults.NoContent();
    }
}
