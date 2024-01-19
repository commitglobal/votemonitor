using Vote.Monitor.Api.Feature.CSOAdmin.Specifications;

namespace Vote.Monitor.Api.Feature.CSOAdmin.Delete;

public class Endpoint(IRepository<CSOAdminAggregate> _repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{

    public override void Configure()
    {
        Delete("/api/csos/{csoid}/admins/{id}");
        DontAutoTag();
        Options(x => x.WithTags("cso-admins"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var csoAdmin = await _repository.SingleOrDefaultAsync(new GetCSOAdminByIdSpecification(req.CSOId, req.Id), ct);

        if (csoAdmin == null)
        {
            return TypedResults.NotFound();
        }

        await _repository.DeleteAsync(csoAdmin, ct);
        return TypedResults.NoContent();
    }
}
