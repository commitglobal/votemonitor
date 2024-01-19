namespace Vote.Monitor.Api.Feature.CSO.Delete;

public class Endpoint(IRepository<CSOAggregate> _repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{

    public override void Configure()
    {
        Delete("/api/csos/{id}");
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var cso = await _repository.GetByIdAsync(req.Id, ct);

        if (cso is null)
        {
            return TypedResults.NotFound();
        }

        await _repository.DeleteAsync(cso, ct);

        return TypedResults.NoContent();
    }
}
