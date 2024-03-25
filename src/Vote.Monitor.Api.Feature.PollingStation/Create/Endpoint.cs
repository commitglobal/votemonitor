using Vote.Monitor.Api.Feature.PollingStation.Helpers;
using Vote.Monitor.Api.Feature.PollingStation.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Create;
public class Endpoint(
    IRepository<PollingStationAggregate> repository,
    IRepository<ElectionRoundAggregate> electionRoundRepository)
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
        var specification = new GetPollingStationSpecification(req.Address, req.Tags);
        var hasIdenticalPollingStation = await repository.AnyAsync(specification, ct);

        if (hasIdenticalPollingStation)
        {
            AddError("A polling station with same address and tags exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "A polling station with same address and tags exists");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var pollingStation = new PollingStationAggregate(electionRound, req.Address, req.DisplayOrder, req.Tags.ToTagsObject());
        await repository.AddAsync(pollingStation, ct);

        return TypedResults.Ok(new PollingStationModel
        {
            Id = pollingStation.Id,
            Address = pollingStation.Address,
            DisplayOrder = pollingStation.DisplayOrder,
            Tags = pollingStation.Tags.ToDictionary()
        });
    }
}
