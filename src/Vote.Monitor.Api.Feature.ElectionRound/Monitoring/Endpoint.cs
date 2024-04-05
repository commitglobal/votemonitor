using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Api.Feature.ElectionRound.Monitoring;

public class Endpoint(IReadRepository<MonitoringNgo> repository,
    ICurrentUserRoleProvider roleProvider)
    : EndpointWithoutRequest<Results<Ok<Result>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds:monitoring");
        DontAutoTag();
        Options(x => x.WithTags("election-rounds"));
        Summary(s =>
        {
            s.Summary = "Lists election rounds which are monitored by current NGO";
            s.Description = "Election rounds with status NotStarted and Started are listed";
        });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Result>, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var ngoId = await roleProvider.GetNgoId();
        var electionRounds = await repository.ListAsync(new GetNgoElectionSpecification(ngoId!.Value), ct);

        return TypedResults.Ok(new Result { ElectionRounds = electionRounds });
    }
}
