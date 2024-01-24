using Vote.Monitor.Api.Feature.CSOAdmin.Specifications;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.CSOAdmin.Create;

public class Endpoint : Endpoint<Request, Results<Ok<CSOAdminModel>, Conflict<ProblemDetails>>>
{
    private readonly IRepository<CSOAdminAggregate> _repository;
    private readonly ITimeProvider _timeProvider;

    public Endpoint(IRepository<CSOAdminAggregate> repository, ITimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public override void Configure()
    {
        Post("/api/csos/{csoid}/admins");
        DontAutoTag();
        Options(x => x.WithTags("cso-admins"));
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

        var csoAdmin = new CSOAdminAggregate(req.CSOId, req.Name, req.Login, req.Password, _timeProvider);
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
