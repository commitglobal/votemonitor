using Vote.Monitor.Api.Feature.PollingStation.Helpers;
using Vote.Monitor.Api.Feature.PollingStation.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Update;

public class Endpoint(IRepository<PollingStationAggregate> repository,
    IRepository<ElectionRoundAggregate> electionRoundRepository)
    : Endpoint<Request, Results<NoContent, NotFound<ProblemDetails>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/polling-stations/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
    }

    public override async Task<Results<NoContent, NotFound<ProblemDetails>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "Election round not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var pollingStation = await repository.SingleOrDefaultAsync(new GetPollingStationByIdSpecification(req.ElectionRoundId, req.Id), ct);
        if (pollingStation is null)
        {
            AddError(r => r.Id, "Polling station not found.");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var specification = new GetPollingStationSpecification(req.ElectionRoundId, req.Address, req.Tags);
        var hasIdenticalPollingStation = await repository.AnyAsync(specification, ct);
        if (hasIdenticalPollingStation)
        {
            AddError("A polling station with same address and tags exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        pollingStation.UpdateDetails(req.Level1, req.Level2, req.Level3, req.Level4, req.Level5, req.Number, req.Address, req.DisplayOrder, req.Tags.ToTagsObject());
        await repository.UpdateAsync(pollingStation, ct);
        electionRound.UpdatePollingStationsVersion();
        await electionRoundRepository.UpdateAsync(electionRound, ct);

        return TypedResults.NoContent();
    }
}
