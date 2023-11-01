using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.Feature.PollingStation.Specifications;

namespace Vote.Monitor.Feature.PollingStation.Update;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    private readonly IRepository<Domain.Entities.PollingStationAggregate.PollingStation> _repository;

    public Endpoint(IRepository<Domain.Entities.PollingStationAggregate.PollingStation> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/polling-stations/{id:guid}");
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var pollingStation = await _repository.SingleOrDefaultAsync(new GetPollingStationByIdSpecification(req.Id), ct);

        if (pollingStation is null)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetPollingStationSpecification(req.Address, req.Tags);
        var hasIdenticalPollingStation = await _repository.AnyAsync(specification, ct);

        if (hasIdenticalPollingStation)
        {
            AddError("A polling station with same address and tags exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        pollingStation.UpdateDetails(req.Address, req.DisplayOrder, req.Tags.ToTagsObject());
        await _repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
