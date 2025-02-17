using Feature.FormTemplates.Specifications;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.Update;

public class Endpoint(IRepository<FormTemplate> repository)
    : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/form-templates/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormTemplateSpecification(req.Id, req.Code, req.FormType);
        var duplicatedFormTemplate = await repository.AnyAsync(specification, ct);

        if (duplicatedFormTemplate)
        {
            AddError(r => r.Name, "A form template with same parameters already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var questions = req.Questions.Select(QuestionsMapper.ToEntity)
            .ToList()
            .AsReadOnly();
        formTemplate.UpdateDetails(req.Code, req.Name, req.Description, req.FormType, req.DefaultLanguage,
            req.Languages, req.Icon, questions);

        await repository.UpdateAsync(formTemplate, ct);
        return TypedResults.NoContent();
    }
}
