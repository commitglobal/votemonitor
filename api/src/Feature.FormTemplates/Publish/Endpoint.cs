using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.Publish;

public class Endpoint(IRepository<FormTemplate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/form-templates/{id}:publish");
        Description(x => x.Accepts<Request>());
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        var result = formTemplate.Publish();

        if (result is PublishFormResult.Error validationResult)
        {
            validationResult.Problems.Errors.ForEach(AddError);
            return new ProblemDetails(ValidationFailures);
        }

        await repository.UpdateAsync(formTemplate, ct);
        return TypedResults.NoContent();
    }
}
