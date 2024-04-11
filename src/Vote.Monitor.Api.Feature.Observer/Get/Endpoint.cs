namespace Vote.Monitor.Api.Feature.Observer.Get;

public class Endpoint(IReadRepository<ObserverAggregate> repository)
    : Endpoint<Request, Results<Ok<ObserverModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/observers/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<ObserverModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var observer = await repository.GetByIdAsync(req.Id, ct);
        if (observer is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new ObserverModel
        {
            Id = observer.Id,
            Login = observer.Login,
            Name = observer.Name,
            PhoneNumber = observer.PhoneNumber,
            Status = observer.Status,
            CreatedOn = observer.CreatedOn,
            LastModifiedOn = observer.LastModifiedOn
        });
    }
}
