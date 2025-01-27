using Authorization.Policies;

namespace Vote.Monitor.Api.Feature.Ngo.Deactivate;

public class Endpoint(IRepository<NgoAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/ngos/{id}:deactivate");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("ngos"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var ngo = await repository.GetByIdAsync(req.Id, ct);

        if (ngo is null)
        {
            return TypedResults.NotFound();
        }

        ngo.Deactivate();

        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
