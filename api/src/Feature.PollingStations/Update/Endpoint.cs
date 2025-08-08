using Feature.PollingStations.Specifications;
using Vote.Monitor.Core.Helpers;

namespace Feature.PollingStations.Update;

public class Endpoint(IRepository<PollingStationAggregate> repository,
    IRepository<ElectionRoundAggregate> electionRoundRepository)
    : Endpoint<Request, Results<NoContent, NotFound<ProblemDetails>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/polling-stations/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
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

        var pollingStation = await repository.SingleOrDefaultAsync(new GetPollingStationByIdSpecification(req.ElectionRoundId, req.Id), ct);
        if (pollingStation is null)
        {
            AddError(r => r.Id, "Polling station not found.");
            return TypedResults.NotFound(new ProblemDetails(ValidationFailures));
        }
        
        pollingStation.UpdateDetails(req.Level1, req.Level2, req.Level3, req.Level4, req.Level5, req.Number, req.Address, req.DisplayOrder, req.Tags.ToTagsObject(), req.Latitude,req.Longitude);
        await repository.UpdateAsync(pollingStation, ct);
        electionRound.UpdatePollingStationsVersion();
        await electionRoundRepository.UpdateAsync(electionRound, ct);

        return TypedResults.NoContent();
    }
}
