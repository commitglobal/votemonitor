namespace Feature.ElectionRounds.Start;

public class Endpoint(IRepository<ElectionRoundAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{id}:start");
        Description(x => x.Accepts<Request>());
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.GetByIdAsync(req.Id, ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        electionRound.Start();

        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
