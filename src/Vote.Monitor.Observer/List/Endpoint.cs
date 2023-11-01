using FastEndpoints;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.Observer.Specifications;

namespace Vote.Monitor.Observer.List;

public class Endpoint : Endpoint<Request, PagedResponse<ObserverModel>>
{
     readonly IReadRepository<Domain.Entities.ApplicationUserAggregate.Observer> _repository;

    public Endpoint(IReadRepository<Domain.Entities.ApplicationUserAggregate.Observer> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/observers");
    }

    public override async Task<PagedResponse<ObserverModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListObserversSpecification(req.NameFilter, req.Status, req.PageSize, req.PageNumber);
        var observers = await _repository.ListAsync(specification, ct);
        var observersCount = await _repository.CountAsync(specification, ct);
        var result = observers.Select(x => new ObserverModel
        {
            Id = x.Id,
            Name = x.Name,
            Login = x.Login,
            Status = x.Status
        }).ToList();

        return new PagedResponse<ObserverModel>(result, observersCount, req.PageNumber, req.PageSize);
    }
}
