using Vote.Monitor.Api.Feature.PollingStation.Helpers;
using Vote.Monitor.Api.Feature.PollingStation.Specifications;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.PollingStation.Create;
public class Endpoint(IRepository<PollingStationAggregate> repository,
    IRepository<ElectionRoundAggregate> electionRoundRepository,
    ITimeProvider timeProvider,
    ICurrentUserProvider userProvider)
    : Endpoint<Request, Results<Ok<PollingStationModel>, Conflict<ProblemDetails>, NotFound<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("api/election-rounds/{electionRoundId}/polling-stations");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
    }

    public override async Task<Results<Ok<PollingStationModel>, Conflict<ProblemDetails>, NotFound<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetPollingStationSpecification(req.ElectionRoundId, req.Address, req.Tags);
        var hasIdenticalPollingStation = await repository.AnyAsync(specification, ct);

        if (hasIdenticalPollingStation)
        {
            AddError("A polling station with same address and tags exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "Election round not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var pollingStation = PollingStationAggregate.Create(electionRound,
            req.Level1,
            req.Level2,
            req.Level3,
            req.Level4,
            req.Level5,
            req.Number,
            req.Address,
            req.DisplayOrder,
            req.Tags.ToTagsObject(),
            timeProvider.UtcNow,
            userProvider.GetUserId()!.Value);

        await repository.AddAsync(pollingStation, ct);
        electionRound.UpdatePollingStationsVersion();
        await electionRoundRepository.UpdateAsync(electionRound, ct);

        return TypedResults.Ok(new PollingStationModel
        {
            Id = pollingStation.Id,
            Level1 = pollingStation.Level1,
            Level2 = pollingStation.Level2,
            Level3 = pollingStation.Level3,
            Level4 = pollingStation.Level4,
            Level5 = pollingStation.Level5,
            Number = pollingStation.Number,
            Address = pollingStation.Address,
            DisplayOrder = pollingStation.DisplayOrder,
            Tags = pollingStation.Tags.ToDictionary()
        });
    }
}
