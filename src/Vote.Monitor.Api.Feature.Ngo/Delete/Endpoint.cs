namespace Vote.Monitor.Api.Feature.Ngo.Delete;

public class Endpoint(IRepository<NgoAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/ngos/{id}");
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var ngo = await repository.GetByIdAsync(req.Id, ct);

        if (ngo is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(ngo, ct);

        return TypedResults.NoContent();
    }
}
