using Feature.Locations.Specifications;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;

namespace Feature.Locations.Create;
public class Endpoint(IRepository<LocationAggregate> repository,
    IRepository<ElectionRoundAggregate> electionRoundRepository,
    ITimeProvider timeProvider,
    ICurrentUserProvider userProvider)
    : Endpoint<Request, Results<Ok<LocationModel>, Conflict<ProblemDetails>, NotFound<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("api/election-rounds/{electionRoundId}/locations");
        DontAutoTag();
        Options(x => x.WithTags("locations"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<LocationModel>, Conflict<ProblemDetails>, NotFound<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetLocationSpecification(req.ElectionRoundId, req.Level1, req.Level2, req.Level3,
            req.Level4, req.Level5);
        var haIdenticalLocation = await repository.AnyAsync(specification, ct);

        if (haIdenticalLocation)
        {
            AddError("A location with same address and tags exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "Election round not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var location = LocationAggregate.Create(electionRound,
            req.Level1,
            req.Level2,
            req.Level3,
            req.Level4,
            req.Level5,
            req.DisplayOrder,
            req.Tags.ToTagsObject(),
            timeProvider.UtcNow,
            userProvider.GetUserId()!.Value);

        await repository.AddAsync(location, ct);
        electionRound.UpdateLocationsVersion();
        await electionRoundRepository.UpdateAsync(electionRound, ct);

        return TypedResults.Ok(new LocationModel
        {
            Id = location.Id,
            Level1 = location.Level1,
            Level2 = location.Level2,
            Level3 = location.Level3,
            Level4 = location.Level4,
            Level5 = location.Level5,
            DisplayOrder = location.DisplayOrder,
            Tags = location.Tags.ToDictionary()
        });
    }
}
