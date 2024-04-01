using Vote.Monitor.Core.Services.Security;

namespace Vote.Monitor.Api.Feature.ElectionRound.My;

public class Endpoint(IReadRepository<ElectionRoundAggregate> repository, ICurrentUserProvider userProvider)
    : EndpointWithoutRequest<Results<Ok<Result>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds:my");
        DontAutoTag();
        Options(x => x.WithTags("election-rounds", "mobile"));
        Summary(s =>
        {
            s.Summary = "Lists election rounds for current user";
            s.Description = "Election rounds with status NotStarted and Started are listed";
        });
    }

    public override async Task<Results<Ok<Result>, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        List<ElectionRoundModel> electionRounds = [];
        if (userProvider.IsPlatformAdmin())
        {
            electionRounds = await repository.ListAsync(new GetElectionsSpecification(), ct);
        }

        if (userProvider.IsNgoAdmin())
        {
            electionRounds = await repository.ListAsync(new GetNgoElectionSpecification(userProvider.GetNgoId()!.Value), ct);
        }

        if (userProvider.IsObserver())
        {
            electionRounds = await repository.ListAsync(new GetObserverElectionSpecification(userProvider.GetUserId()!.Value), ct);
        }

        return TypedResults.Ok(new Result { ElectionRounds = electionRounds });
    }
}
