using Vote.Monitor.Api.Feature.Ngo.Specifications;

namespace Vote.Monitor.Api.Feature.Ngo.Update;

public class Endpoint(IRepository<NgoAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/ngos/{id}");
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var ngo = await repository.GetByIdAsync(req.Id, ct);

        if (ngo is null)
        {
            return TypedResults.NotFound();
        }

        var hasNgoWithSameName = await repository.AnyAsync(new GetNgoByNameSpecification(req.Name), ct);

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
