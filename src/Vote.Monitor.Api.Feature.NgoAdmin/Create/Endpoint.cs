using Vote.Monitor.Api.Feature.NgoAdmin.Specifications;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.NgoAdmin.Create;

public class Endpoint : Endpoint<Request, Results<Ok<NgoAdminModel>, Conflict<ProblemDetails>>>
{
    private readonly IRepository<NgoAdminAggregate> _repository;
    private readonly ITimeProvider _timeProvider;

    public Endpoint(IRepository<NgoAdminAggregate> repository, ITimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public override void Configure()
    {
        Post("/api/ngos/{ngoId}/admins");
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
    }

    public override async Task<Results<Ok<NgoAdminModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetNgoAdminByLoginSpecification(req.NgoId, req.Login);
        var hasNgoAdminWithSameName = await _repository.AnyAsync(specification, ct);

        if (hasNgoAdminWithSameName)
        {
            AddError(r => r.Name, "A ngo admin with same login already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var ngoAdmin = new NgoAdminAggregate(req.NgoId, req.Name, req.Login, req.Password, _timeProvider);
        await _repository.AddAsync(ngoAdmin, ct);

        return TypedResults.Ok(new NgoAdminModel
        {
            Id = ngoAdmin.Id,
            Name = ngoAdmin.Name,
            Login = ngoAdmin.Login,
            Status = ngoAdmin.Status,
            CreatedOn = ngoAdmin.CreatedOn,
            LastModifiedOn = ngoAdmin.LastModifiedOn
        });

    }
}
