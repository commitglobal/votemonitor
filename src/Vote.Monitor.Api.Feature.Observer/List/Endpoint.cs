﻿using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.Observer.List;

public class Endpoint : Endpoint<Request, PagedResponse<ObserverModel>>
{
    readonly IReadRepository<ObserverAggregate> _repository;

    public Endpoint(IReadRepository<ObserverAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/observers");
    }

    public override async Task<PagedResponse<ObserverModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListObserversSpecification(req);
        var observers = await _repository.ListAsync(specification, ct);
        var observersCount = await _repository.CountAsync(specification, ct);
        var result = observers.Select(x => new ObserverModel
        {
            Id = x.Id,
            Name = x.Name,
            Login = x.Login,
            PhoneNumber = x.PhoneNumber,
            Status = x.Status,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn
        }).ToList();

        return new PagedResponse<ObserverModel>(result, observersCount, req.PageNumber, req.PageSize);
    }
}
