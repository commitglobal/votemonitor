namespace Feature.ElectionRounds.Delete;

public class Endpoint(IRepository<ElectionRoundAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.GetByIdAsync(req.Id, ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(electionRound, ct);

        return TypedResults.NoContent();
    }
}
