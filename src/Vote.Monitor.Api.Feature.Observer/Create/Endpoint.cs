namespace Vote.Monitor.Api.Feature.Observer.Create;

public class Endpoint(IRepository<ObserverAggregate> repository)
    : Endpoint<Request, Results<Ok<ObserverModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/observers");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<ObserverModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetObserverByLoginSpecification(req.Email);
        var hasObserverWithSameLogin = await repository.AnyAsync(specification, ct);

        if (hasObserverWithSameLogin)
        {
            AddError(r => r.Name, "A observer with same login already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var observer = ObserverAggregate.Create(req.Name, req.Email, req.Password, req.PhoneNumber);
        await repository.AddAsync(observer, ct);

        return TypedResults.Ok(new ObserverModel
        {
            Id = observer.Id,
            Name = observer.Name,
            Login = observer.Login,
            PhoneNumber = observer.PhoneNumber,
            Status = observer.Status,
            CreatedOn = observer.CreatedOn,
            LastModifiedOn = observer.LastModifiedOn
        });
    }
}
