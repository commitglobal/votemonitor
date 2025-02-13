using Feature.FormTemplates.Specifications;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.Create;

public class Endpoint(IRepository<FormTemplate> repository) :
        Endpoint<Request, Results<Ok<FormTemplateFullModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/form-templates");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<FormTemplateFullModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormTemplateSpecification(req.Code, req.FormType);
        var duplicatedFormTemplate = await repository.AnyAsync(specification, ct);

        if (duplicatedFormTemplate)
        {
            AddError(r => r.Code, "A form template with same parameters already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }
        
        var questions = req.Questions.Select(QuestionsMapper.ToEntity)
            .ToList()
            .AsReadOnly();
        
        var formTemplate = FormTemplate.Create(req.FormType, req.Code, req.DefaultLanguage, req.Name, req.Description, req.Languages, req.Icon, questions);

        await repository.AddAsync(formTemplate, ct);

        return TypedResults.Ok(FormTemplateFullModel.FromEntity(formTemplate));
    }
}
