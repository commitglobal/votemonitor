using Vote.Monitor.Api.Feature.FormTemplate.Specifications;
using Vote.Monitor.Form.Module.Mappers;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update;

public class Endpoint(IRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/form-templates/{id}");
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormTemplateSpecification(req.Id, req.Code, req.FormTemplateType);
        var duplicatedFormTemplate = await repository.AnyAsync(specification, ct);

        if (duplicatedFormTemplate)
        {
            AddError(r => r.Name, "A form template with same parameters already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var questions = req.Questions.Select(FormMapper.ToEntity)
                 .ToList()
                 .AsReadOnly();
        formTemplate.UpdateDetails(req.Code, req.DefaultLanguage, req.Name, req.FormTemplateType, req.Languages, questions);

        await repository.UpdateAsync(formTemplate, ct);
        return TypedResults.NoContent();
    }
}
