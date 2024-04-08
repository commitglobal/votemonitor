namespace Vote.Monitor.Api.Feature.ElectionRound.Create;

public class Endpoint(IRepository<ElectionRoundAggregate> repository)
    : Endpoint<Request, Results<Ok<ElectionRoundModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<ElectionRoundModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetActiveElectionRoundSpecification(req.CountryId, req.Title);
        var hasElectionRoundWithSameTitle = await repository.AnyAsync(specification, ct);

        if (hasElectionRoundWithSameTitle)
        {
            AddError(r => r.Title, "An election round with same title already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var electionRound = new ElectionRoundAggregate(req.CountryId, req.Title,req.EnglishTitle, req.StartDate);
        await repository.AddAsync(electionRound, ct);

        return TypedResults.Ok(new ElectionRoundModel
        {
            Id = electionRound.Id,
            Title = electionRound.Title,
            EnglishTitle = electionRound.EnglishTitle,
            StartDate = electionRound.StartDate,
            Status = electionRound.Status,
            CreatedOn = electionRound.CreatedOn,
            LastModifiedOn = electionRound.LastModifiedOn,
            Country = CountriesList.Get(req.CountryId)!.FullName,
            CountryId = req.CountryId
        });
    }
}
