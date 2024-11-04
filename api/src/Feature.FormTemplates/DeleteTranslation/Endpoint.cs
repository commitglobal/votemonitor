using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.DeleteTranslation;

public class Endpoint(IRepository<Form> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/form-templates/{id}/{languageCode}");
        Description(x => x.Accepts<Request>());
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(x =>
        {
            x.Description = "Removes supported language from a form template";
            x.Description = "Translations for removed language will be removed as well.";
        });
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        formTemplate.RemoveTranslation(req.LanguageCode);

        await repository.UpdateAsync(formTemplate, ct);

        return TypedResults.NoContent();
    }
}
