using Vote.Monitor.Api.Feature.PollingStation.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Delete;
public class Endpoint(IRepository<PollingStationAggregate> repository,
    IRepository<ElectionRoundAggregate> electionRoundRepository)
    : Endpoint<Request, Results<NoContent, NotFound<ProblemDetails>, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-stations/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
    }

    public override async Task<Results<NoContent, NotFound<ProblemDetails>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await electionRoundRepository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            AddError(r => r.ElectionRoundId, "Election round not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        var pollingStation = await repository.FirstOrDefaultAsync(new GetPollingStationByIdSpecification(req.ElectionRoundId, req.Id), ct);

        if (pollingStation is null)
        {
            AddError(r => r.Id, "Polling station not found");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }

        await repository.DeleteAsync(pollingStation, ct);

        electionRound.UpdatePollingStationsVersion();
        await electionRoundRepository.UpdateAsync(electionRound, ct);

        return TypedResults.NoContent();
    }
}
