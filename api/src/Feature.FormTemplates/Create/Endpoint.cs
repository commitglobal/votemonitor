using Feature.FormTemplates.Specifications;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.Create;

public class Endpoint(IRepository<FormTemplate> repository) :
    Endpoint<Request, Ok<FormTemplateFullModel>>
{
    public override void Configure()
    {
        Post("/api/form-templates");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Ok<FormTemplateFullModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var questions = req.Questions.Select(QuestionsMapper.ToEntity)
            .ToList()
            .AsReadOnly();

        var formTemplate = FormTemplate.Create(req.FormType, req.Code, req.DefaultLanguage, req.Name, req.Description,
            req.Languages, req.Icon, questions);

        await repository.AddAsync(formTemplate, ct);

        return TypedResults.Ok(FormTemplateFullModel.FromEntity(formTemplate));
    }
}
