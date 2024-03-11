using Vote.Monitor.Api.Feature.FormTemplate.Models;
using Vote.Monitor.Api.Feature.FormTemplate.Specifications;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.FormTemplate.Create;

public class Endpoint(IRepository<FormTemplateAggregate> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Results<Ok<FormTemplateModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/form-templates");
    }

    public override async Task<Results<Ok<FormTemplateModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormTemplateSpecification(req.Code, req.FormType);
        var duplicatedFormTemplate = await repository.AnyAsync(specification, ct);

        if (duplicatedFormTemplate)
        {
            AddError(r => r.Code, "A form template with same parameters already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var formTemplate = FormTemplateAggregate.Create(req.FormType, req.Code, req.Name, req.Languages, timeProvider);

        await repository.AddAsync(formTemplate, ct);

        return TypedResults.Ok(new FormTemplateModel
        {
            Id = formTemplate.Id,
            Code = formTemplate.Code,
            Name = formTemplate.Name,
            Status = formTemplate.Status,
            CreatedOn = formTemplate.CreatedOn,
            LastModifiedOn = formTemplate.LastModifiedOn,
            Sections = []
        });
    }
}
