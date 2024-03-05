namespace Vote.Monitor.Api.Feature.FormTemplate.Draft;

public class Endpoint(IRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/form-templates/{id}:draft");
        Description(x => x.Accepts<Request>());
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
