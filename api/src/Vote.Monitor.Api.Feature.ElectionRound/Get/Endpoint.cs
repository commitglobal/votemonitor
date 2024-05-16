namespace Vote.Monitor.Api.Feature.ElectionRound.Get;

public class Endpoint(IReadRepository<ElectionRoundAggregate> repository)
    : Endpoint<Request, Results<Ok<ElectionRoundModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{id}");
        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task<Results<Ok<ElectionRoundModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetElectionRoundByIdSpecification(req.Id);
        var electionRound = await repository.SingleOrDefaultAsync(specification, ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(electionRound);
    }
}
