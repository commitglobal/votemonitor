using Vote.Monitor.Api.Feature.FormTemplate.Specifications;
using Vote.Monitor.Domain.Entities.FormBase;
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

        var sections = req.Sections.Select(section =>
         {
             var questions = section.Questions
                 .Select(FormMapper.ToEntity)
                 .ToList()
                 .AsReadOnly();

             return FormSection.Create(section.Code, section.Title, questions);
         })
         .ToList()
         .AsReadOnly();

        formTemplate.UpdateDetails(req.Code, req.Name, req.FormTemplateType, req.Languages, sections);

        await repository.UpdateAsync(formTemplate, ct);
        return TypedResults.NoContent();
    }
}
