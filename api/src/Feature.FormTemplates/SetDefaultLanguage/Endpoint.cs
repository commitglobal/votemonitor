namespace Feature.FormTemplates.SetDefaultLanguage;

public class Endpoint(IRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/form-templates/{id}:setDefaultLanguage");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        if (!formTemplate.HasTranslation(req.LanguageCode))
        {
            AddError(x => x.LanguageCode, "Form template does not have translations for language code");
        }
        formTemplate.SetDefaultLanguage(req.LanguageCode);

        await repository.UpdateAsync(formTemplate, ct);

        return TypedResults.NoContent();
    }
}
