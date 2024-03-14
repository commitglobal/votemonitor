namespace Vote.Monitor.Api.Feature.Form.Delete;

public class Endpoint(IRepository<FormAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgo}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(formTemplate, ct);

        return TypedResults.NoContent();
    }
}
