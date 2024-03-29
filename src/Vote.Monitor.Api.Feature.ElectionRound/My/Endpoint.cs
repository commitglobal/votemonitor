using Vote.Monitor.Core.Services.Security;

namespace Vote.Monitor.Api.Feature.ElectionRound.My;

public class Endpoint(IReadRepository<ElectionRoundAggregate> repository,
    ICurrentUserProvider userProvider,
    ITimeProvider timeProvider)
    : EndpointWithoutRequest<Results<Ok<Result>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds:upcoming");
        DontAutoTag();
        Options(x => x.WithTags("election-rounds", "mobile"));
    }

    public override async Task<Results<Ok<Result>, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        List<ElectionRoundModel> electionRounds = [];
        if (userProvider.IsPlatformAdmin())
        {
            electionRounds = await repository.ListAsync(new GetUpcomingElectionSpecification(timeProvider), ct);
        }

        if (userProvider.IsNgoAdmin())
        {
            electionRounds = await repository.ListAsync(new GetNgoUpcomingElectionSpecification(userProvider.GetNgoId()!.Value, timeProvider), ct);
        }

        if (userProvider.IsObserver())
        {
            electionRounds = await repository.ListAsync(new GetObserverUpcomingElectionSpecification(userProvider.GetUserId()!.Value, timeProvider), ct);
        }

        return TypedResults.Ok(new Result { ElectionRounds = electionRounds });
    }
}
