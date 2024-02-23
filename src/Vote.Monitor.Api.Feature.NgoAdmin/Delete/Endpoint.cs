using Vote.Monitor.Api.Feature.NgoAdmin.Specifications;

namespace Vote.Monitor.Api.Feature.NgoAdmin.Delete;

public class Endpoint(IRepository<NgoAdminAggregate> _repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/ngos/{ngoId}/admins/{id}");
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var ngoAdmin = await _repository.SingleOrDefaultAsync(new GetNgoAdminByIdSpecification(req.NgoId, req.Id), ct);

        if (ngoAdmin == null)
        {
            return TypedResults.NotFound();
        }

        await _repository.DeleteAsync(ngoAdmin, ct);
        return TypedResults.NoContent();
    }
}
