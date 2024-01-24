namespace Vote.Monitor.Api.Feature.ElectionRound.List;

public class Endpoint(IReadRepository<ElectionRoundAggregate> repository)
    : Endpoint<Request, Results<Ok<PagedResponse<ElectionRoundBaseModel>>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds");
    }

    public override async Task<Results<Ok<PagedResponse<ElectionRoundBaseModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListElectionRoundsSpecification(req);
        var electionRounds = await repository.ListAsync(specification, ct);
        var electionRoundsCunt = await repository.CountAsync(specification, ct);

        return TypedResults.Ok(new PagedResponse<ElectionRoundBaseModel>(electionRounds, electionRoundsCunt, req.PageNumber, req.PageSize));
    }
}
