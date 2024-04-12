namespace Vote.Monitor.Api.Feature.Observer.List;

public class Endpoint(IReadRepository<ObserverAggregate> repository) : Endpoint<Request, PagedResponse<ObserverModel>>
{
    public override void Configure()
    {
        Get("/api/observers");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<PagedResponse<ObserverModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListObserversSpecification(req);
        var observers = await repository.ListAsync(specification, ct);
        var observersCount = await repository.CountAsync(specification, ct);
        var result = observers.Select(x => new ObserverModel
        {
            Id = x.Id,
            Email = x.ApplicationUser.Email!,
            FirstName = x.ApplicationUser.FirstName,
            LastName = x.ApplicationUser.LastName,
            PhoneNumber = x.ApplicationUser.PhoneNumber!,
            Status = x.ApplicationUser.Status,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn
        }).ToList();

        return new PagedResponse<ObserverModel>(result, observersCount, req.PageNumber, req.PageSize);
    }
}
