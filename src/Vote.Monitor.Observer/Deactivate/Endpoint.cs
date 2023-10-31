using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Observer.Deactivate;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
     readonly IRepository<Domain.Entities.ApplicationUserAggregate.Observer> _repository;

    public Endpoint(IRepository<Domain.Entities.ApplicationUserAggregate.Observer> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/observers/{id:guid}:deactivate");
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var observer = await _repository.GetByIdAsync(req.Id, ct);
        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        observer.Deactivate();
        await _repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
