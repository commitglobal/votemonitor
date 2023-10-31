using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Observer.Export;

public class Endpoint : EndpointWithoutRequest<Results<Ok<ObserverModel>, NotFound>>
{
     readonly IReadRepository<Domain.Entities.ApplicationUserAggregate.Observer> _repository;

    public Endpoint(IReadRepository<Domain.Entities.ApplicationUserAggregate.Observer> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/observers:export");
    }

    public override async Task<Results<Ok<ObserverModel>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
