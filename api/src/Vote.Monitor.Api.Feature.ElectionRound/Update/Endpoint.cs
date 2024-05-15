namespace Vote.Monitor.Api.Feature.ElectionRound.Update;

public class Endpoint(IRepository<ElectionRoundAggregate> repository)
    : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.GetByIdAsync(req.Id, ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetActiveElectionRoundSpecification(req.CountryId, req.Title);
        var hasElectionRoundWithSameTitle = await repository.AnyAsync(specification, ct);
        if (hasElectionRoundWithSameTitle)
        {
            AddError(r => r.Title, "An election round with same title already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        electionRound.UpdateDetails(req.CountryId, req.Title, req.EnglishTitle, req.StartDate);
        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
