using Vote.Monitor.Api.Feature.PollingStation.Helpers;
using Vote.Monitor.Api.Feature.PollingStation.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Update;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    private readonly IRepository<PollingStationAggregate> _repository;

    public Endpoint(IRepository<PollingStationAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/polling-stations/{id}");
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
