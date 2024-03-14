namespace Vote.Monitor.Api.Feature.Form.Draft;

public class Endpoint(IRepository<FormAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgo}/forms/{id}:draft");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        formTemplate.Draft();

        await repository.UpdateAsync(formTemplate, ct);

        return TypedResults.NoContent();
    }
}
