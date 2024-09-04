using Authorization.Policies;
using Vote.Monitor.Api.Feature.Monitoring.Specifications;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.EnableCitizenReportingNgo;

public class Endpoint(
    IRepository<ElectionRoundAggregate> repository,
    IReadRepository<NgoAggregate> ngoRepository,
    IRepository<MonitoringNgoAggregate> monitoringNgoRepository)
    : Endpoint<Request, Results<NoContent, NotFound<string>, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}:enableCitizenReporting");
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound<string>, ValidationProblem>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var electionRound = await repository.FirstOrDefaultAsync(new GetElectionRoundByIdSpecification(req.ElectionRoundId), ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
        }

        if (electionRound.Status == ElectionRoundStatus.Archived)
        {
            AddError(x => x.ElectionRoundId, "Election round is archived");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        if (electionRound.CitizenReportingEnabled)
        {
            return TypedResults.NoContent();
        }

        var ngo = await ngoRepository.GetByIdAsync(req.NgoId, ct);
        if (ngo is null)
        {
            return TypedResults.NotFound("NGO not found");
        }

        if (ngo.Status == NgoStatus.Deactivated)
        {
            AddError(x => x.NgoId, "Only active ngos can monitor elections");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var monitoringNgo = electionRound.MonitoringNgos.FirstOrDefault(x => x.NgoId == req.NgoId);
        if (monitoringNgo is null)
        {
            monitoringNgo = electionRound.AddMonitoringNgo(ngo);
            await monitoringNgoRepository.AddAsync(monitoringNgo, ct);
        }

        electionRound.EnableCitizenReporting(monitoringNgo);
        await repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}