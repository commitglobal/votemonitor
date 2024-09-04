using Vote.Monitor.Api.Feature.Monitoring.Specifications;

namespace Vote.Monitor.Api.Feature.Monitoring.DisableCitizenReportingNgo;

public class Endpoint(IRepository<ElectionRoundAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound<string>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}:disableCitizenReporting");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
    }

    public override async Task<Results<NoContent, NotFound<string>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.SingleOrDefaultAsync(new GetElectionRoundByIdSpecification(req.ElectionRoundId), ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
        }
        
        electionRound.DisableCitizenReporting();

        await repository.UpdateAsync(electionRound, ct);
        return TypedResults.NoContent();
    }
}
