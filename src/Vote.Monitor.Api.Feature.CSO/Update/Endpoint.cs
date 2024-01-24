using Vote.Monitor.Api.Feature.CSO.Specifications;

namespace Vote.Monitor.Api.Feature.CSO.Update;

public class Endpoint(IRepository<CSOAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/csos/{id}");
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var CSO = await repository.GetByIdAsync(req.Id, ct);

        if (CSO is null)
        {
            return TypedResults.NotFound();
        }

        var hasCSOWithSameName = await repository.AnyAsync(new GetCSOByNameSpecification(req.Name), ct);

        if (hasCSOWithSameName)
        {
            AddError(r => r.Name, "A CSO with same name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        CSO.UpdateDetails(req.Name);
        await repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
