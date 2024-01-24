using Vote.Monitor.Api.Feature.CSO.Specifications;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.CSO.Create;

public class Endpoint : Endpoint<Request, Results<Ok<CSOModel>, Conflict<ProblemDetails>>>
{
    private readonly IRepository<CSOAggregate> _repository;
    private readonly ITimeProvider _timeProvider;

    public Endpoint(IRepository<CSOAggregate> repository,
        ITimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public override void Configure()
    {
        Post("/api/csos");
    }

    public override async Task<Results<Ok<CSOModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOByNameSpecification(req.Name);
        var hasCSOWithSameName = await _repository.AnyAsync(specification, ct);

        if (hasCSOWithSameName)
        {
            AddError(r => r.Name, "A CSO with same name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var cso = new CSOAggregate(req.Name, _timeProvider);
        await _repository.AddAsync(cso, ct);

        return TypedResults.Ok(new CSOModel
        {
            Id = cso.Id,
            Name = cso.Name,
            Status = cso.Status,
            CreatedOn = cso.CreatedOn,
            LastModifiedOn = cso.LastModifiedOn
        });
    }
}
