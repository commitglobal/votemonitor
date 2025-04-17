namespace Feature.ElectionRounds.ObservingV2;

public class Endpoint(IReadRepository<ElectionRoundAggregate> repository)
    : Endpoint<Request, Results<Ok<Result>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds:myV2");
        DontAutoTag();
        Options(x => x.WithTags("election-rounds", "mobile"));
        Summary(s =>
        {
            s.Summary = "Lists election rounds which are observed by current observer";
            s.Description = "Election rounds with status Archived and Started are listed";
        });
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok<Result>, ProblemDetails>> ExecuteAsync(Request request, CancellationToken ct)
    {
        List<ElectionRoundModel> electionRounds =
            await repository.ListAsync(new GetObserverElectionV2Specification(request.UserId), ct);

        return TypedResults.Ok(new Result { ElectionRounds = electionRounds });
    }
}
