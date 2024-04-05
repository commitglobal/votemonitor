using Vote.Monitor.Core.Services.Security;

namespace Vote.Monitor.Api.Feature.ElectionRound.My;

public class Endpoint(IReadRepository<ElectionRoundAggregate> repository, 
    ICurrentUserRoleProvider currentUserRoleProvider,
    ICurrentUserIdProvider currentUserIdProvider)
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
        if (currentUserRoleProvider.IsPlatformAdmin())
        {
            electionRounds = await repository.ListAsync(new GetElectionsSpecification(), ct);
        }

        if (currentUserRoleProvider.IsNgoAdmin())
        {
            var ngoId = await currentUserRoleProvider.GetNgoId();
            electionRounds = await repository.ListAsync(new GetNgoElectionSpecification(ngoId!.Value), ct);
        }

        if (currentUserRoleProvider.IsObserver())
        {
            electionRounds = await repository.ListAsync(new GetObserverElectionSpecification(currentUserIdProvider.GetUserId()!.Value), ct);
        }

        return TypedResults.Ok(new Result { ElectionRounds = electionRounds });
    }
}
