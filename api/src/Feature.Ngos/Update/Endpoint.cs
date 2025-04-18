using Authorization.Policies;
using Feature.Ngos.Specifications;

namespace Feature.Ngos.Update;

public class Endpoint(IRepository<NgoAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/ngos/{id}");
        DontAutoTag();
        Options(x => x.WithTags("ngos"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var ngo = await repository.GetByIdAsync(req.Id, ct);

        if (ngo is null)
        {
            return TypedResults.NotFound();
        }

        var hasNgoWithSameName = await repository.AnyAsync(new GetNgoWithSameNameSpecification(req.Id, req.Name), ct);

        if (hasNgoWithSameName)
        {
            AddError(r => r.Name, "A ngo with same name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        ngo.UpdateDetails(req.Name);
        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
