using Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

namespace Vote.Monitor.Api.Feature.CSOAdmin.Deactivate;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
     readonly IRepository<CSOAdminAggregate> _repository;

    public Endpoint(IRepository<CSOAdminAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/csos/{csoid}/admins/{id}:deactivate");
        DontAutoTag();
        Options(x => x.WithTags("cso-admins"));
        Description(x => x.Accepts<Request>());
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetCSOAdminByIdSpecification(req.CSOId, req.Id);
        var csoAdmin = await _repository.SingleOrDefaultAsync(specification, ct);

        if (csoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        csoAdmin.Deactivate();
        await _repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
