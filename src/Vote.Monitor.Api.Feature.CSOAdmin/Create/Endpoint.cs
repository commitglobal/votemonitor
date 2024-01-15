using Vote.Monitor.Api.Feature.CSOAdmin.Specifications;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.CSOAdmin.Create;

public class Endpoint : Endpoint<Request, Results<Ok<CSOAdminModel>, Conflict<ProblemDetails>>>
{
    private readonly IRepository<CSOAdminAggregate> _repository;
    private readonly ITimeService _timeService;

    public Endpoint(IRepository<CSOAdminAggregate> repository, ITimeService timeService)
    {
        _repository = repository;
        _timeService = timeService;
    }

    public override void Configure()
    {
        Post("/api/csos/{CSOid}/admins");
    }

    public override async Task<Results<Ok<CSOAdminModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdminByLoginSpecification(req.CSOId, req.Login);
        var hasCSOAdminWithSameLogin = await _repository.AnyAsync(specification, ct);

        if (hasCSOAdminWithSameLogin)
        {
            AddError(r => r.Name, "A CSO admin with same login already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var csoAdmin = new CSOAdminAggregate(req.CSOId, req.Name, req.Login, req.Password, _timeService);
        await _repository.AddAsync(csoAdmin, ct);

        return TypedResults.Ok(new CSOAdminModel
        {
            Id = csoAdmin.Id,
            Name = csoAdmin.Name,
            Login = csoAdmin.Login,
            Status = csoAdmin.Status,
            CreatedOn = csoAdmin.CreatedOn,
            LastModifiedOn = csoAdmin.LastModifiedOn
        });

    }
}
