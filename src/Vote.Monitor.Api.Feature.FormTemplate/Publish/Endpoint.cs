using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate.Publish;

public class Endpoint(IRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/form-templates/{id}:publish");
        Description(x => x.Accepts<Request>());
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        var result = formTemplate.Publish();

        if (result is PublishResult.InvalidFormTemplate validationResult)
        {
            validationResult.Problems.Errors.ForEach(AddError);
            return new ProblemDetails(ValidationFailures);
        }

        await repository.UpdateAsync(formTemplate, ct);
        return TypedResults.NoContent();
    }
}
