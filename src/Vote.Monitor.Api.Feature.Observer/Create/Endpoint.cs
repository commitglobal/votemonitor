using Vote.Monitor.Api.Feature.Observer.Specifications;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.Observer.Create;

public class Endpoint : Endpoint<Request, Results<Ok<ObserverModel>, Conflict<ProblemDetails>>>
{
    private readonly IRepository<ObserverAggregate> _repository;
    private readonly ITimeService _timeService;

    public Endpoint(IRepository<ObserverAggregate> repository, ITimeService timeService)
    {
        _repository = repository;
        _timeService = timeService;
    }

    public override void Configure()
    {
        Post("/api/observers");
    }

    public override async Task<Results<Ok<ObserverModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetObserverByLoginSpecification(req.Email);
        var hasObserverWithSameLogin = await _repository.AnyAsync(specification, ct);

        if (hasObserverWithSameLogin)
        {
            AddError(r => r.Name, "A observer with same login already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var observer = new ObserverAggregate(req.Name, req.Email, req.Password, req.PhoneNumber, _timeService);
        await _repository.AddAsync(observer, ct);

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
