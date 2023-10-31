using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Observer.Import;

public class Endpoint : Endpoint<Request, Results<Ok<ObserverModel>, NotFound>>
{
     readonly IReadRepository<Domain.Entities.ApplicationUserAggregate.Observer> _repository;

    public Endpoint(IReadRepository<Domain.Entities.ApplicationUserAggregate.Observer> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/observers:import");
    }

    public override async Task<Results<Ok<ObserverModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
