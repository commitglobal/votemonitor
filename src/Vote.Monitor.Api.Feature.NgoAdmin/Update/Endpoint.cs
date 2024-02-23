using Vote.Monitor.Api.Feature.NgoAdmin.Specifications;

namespace Vote.Monitor.Api.Feature.NgoAdmin.Update;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
{
     readonly IRepository<NgoAdminAggregate> _repository;

    public Endpoint(IRepository<NgoAdminAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/ngos/{ngoId}/admins/{id}");
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetNgoAdminByIdSpecification(req.NgoId, req.Id);
        var ngoAdmin = await _repository.SingleOrDefaultAsync(specification, ct);

        if (ngoAdmin is null)
        {
            return TypedResults.NotFound();
        }

        ngoAdmin.UpdateDetails(req.Name);
        await _repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
