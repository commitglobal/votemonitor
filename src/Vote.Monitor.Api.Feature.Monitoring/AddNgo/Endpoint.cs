using Vote.Monitor.Api.Feature.Monitoring.Specifications;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.AddNgo;

public class Endpoint(IRepository<ElectionRoundAggregate> repository, IReadRepository<NgoAggregate> ngoRepository)
    : Endpoint<Request, Results<NoContent, NotFound<string>, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{id}/monitoring-ngos");
        DontAutoTag();
        Options(x => x.WithTags("monitoring"));
    }

    public override async Task<Results<NoContent, NotFound<string>, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.GetByIdAsync(req.Id, ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
        }

        var ngoStatus = await ngoRepository.SingleOrDefaultAsync(new GetNgoStatusSpecification(req.NgoId), ct);
        if (ngoStatus is null)
        {
            return TypedResults.NotFound("NGO not found");
        }

        if (ngoStatus == NgoStatus.Deactivated)
        {
            AddError(x=>x.NgoId, "Only active ngos can monitor elections");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        electionRound.AddMonitoringNgo(req.NgoId);

        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
