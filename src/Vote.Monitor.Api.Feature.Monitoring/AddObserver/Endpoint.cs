using Vote.Monitor.Api.Feature.Monitoring.Specifications;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.AddObserver;

public class Endpoint(IRepository<ElectionRoundAggregate> repository,
     IReadRepository<NgoAggregate> ngoRepository,
     IReadRepository<ObserverAggregate> observerRepository) : Endpoint<Request, Results<NoContent, NotFound<string>, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{id}/monitoring-observers");
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

        var ngoStatus = await ngoRepository.SingleOrDefaultAsync(new GetNgoStatusSpecification(req.InviterNgoId), ct);
        if (ngoStatus is null)
        {
            return TypedResults.NotFound("NGO not found");
        }

        if (ngoStatus == CSOStatus.Deactivated)
        {
            AddError(x => x.InviterNgoId, "Only active ngos can add monitoring observers");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var observerStatus = await observerRepository.SingleOrDefaultAsync(new GetObserverStatusSpecification(req.ObserverId), ct);
        if (observerStatus is null)
        {
            return TypedResults.NotFound("Observer not found");
        }

        if (observerStatus == UserStatus.Deactivated)
        {
            AddError(x => x.ObserverId, "Only active observers can monitor elections");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        electionRound.AddMonitoringObserver(req.ObserverId, req.InviterNgoId);

        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
