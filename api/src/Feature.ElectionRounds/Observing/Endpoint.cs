using Feature.ElectionRounds.Specifications;
using Vote.Monitor.Core.Services.Security;

namespace Feature.ElectionRounds.Observing;

public class Endpoint(IReadRepository<ElectionRoundAggregate> repository,
    ICurrentUserProvider currentUserProvider)
    : EndpointWithoutRequest<Results<Ok<Result>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds:my");
        DontAutoTag();
        Options(x => x.WithTags("election-rounds", "mobile"));
        Summary(s =>
        {
            s.Summary = "Lists election rounds which are observed by current observer";
            s.Description = "Only election rounds with status Started are listed";
        });
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok<Result>, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        List<ElectionRoundModel> electionRounds = await repository.ListAsync(new GetObserverElectionSpecification(currentUserProvider.GetUserId()!.Value), ct);

        return TypedResults.Ok(new Result { ElectionRounds = electionRounds });
    }
}
