namespace Feature.FormTemplates.AddTranslations;

public class Endpoint(IRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Put("/api/form-templates/{id}:addTranslations");
        Summary(x =>
        {
            x.Description = "Adds supported languages to a form template";
            x.Description = "For each new language a default value will be set in all form template. No duplicated translations will be added.";
        });
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        formTemplate.AddTranslations(req.LanguageCodes);
        await repository.UpdateAsync(formTemplate, ct);

        return TypedResults.NoContent();
    }
}
