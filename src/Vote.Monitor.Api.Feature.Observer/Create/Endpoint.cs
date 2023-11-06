using Vote.Monitor.Api.Feature.Observer.Specifications;

namespace Vote.Monitor.Api.Feature.Observer.Create;

public class Endpoint : Endpoint<Request, Results<Ok<ObserverModel>, Conflict<ProblemDetails>>>
{
     readonly IRepository<ObserverAggregate> _repository;

    public Endpoint(IRepository<ObserverAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/observers");
    }

    public override async Task<Results<Ok<ObserverModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetObserverByLoginSpecification(req.Login);
        var hasObserverWithSameLogin = await _repository.AnyAsync(specification, ct);

        if (hasObserverWithSameLogin)
        {
            AddError(r => r.Name, "A observer with same login already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var observer = new ObserverAggregate(req.Name, req.Login, req.Password);
        await _repository.AddAsync(observer, ct);

        return TypedResults.Ok(new ObserverModel
        {
            Id = observer.Id,
            Name = observer.Name,
            Login = observer.Login,
            Status = observer.Status
        });
    }
}
