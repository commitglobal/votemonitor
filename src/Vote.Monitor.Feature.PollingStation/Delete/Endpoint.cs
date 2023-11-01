using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Feature.PollingStation.Delete;
internal class Endpoint : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    private readonly IRepository<Domain.Entities.PollingStationAggregate.PollingStation> _repository;

    public Endpoint(IRepository<Domain.Entities.PollingStationAggregate.PollingStation> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/api/polling-stations/{id:guid}");
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var pollingStation = await _repository.GetByIdAsync(req.Id, ct);

        if (pollingStation is null)
        {
            return TypedResults.NotFound();
        }

        await _repository.DeleteAsync(pollingStation, ct);

        return TypedResults.NoContent();
    }
}
