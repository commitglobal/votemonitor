using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.AddNgo;

public class Endpoint(IRepository<ElectionRoundAggregate> repository,
    IReadRepository<NgoAggregate> ngoRepository,
    IRepository<MonitoringNgo> monitoringNgoRepository) : Endpoint<Request, Results<NoContent, NotFound<string>, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngos");
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
    }

    public override async Task<Results<NoContent, NotFound<string>, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
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

        var monitoringNgo = electionRound.AddMonitoringNgo(ngo);
        await monitoringNgoRepository.AddAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }
}
