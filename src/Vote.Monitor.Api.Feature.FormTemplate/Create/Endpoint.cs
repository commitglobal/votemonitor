using Vote.Monitor.Api.Feature.FormTemplate.Models;
using Vote.Monitor.Api.Feature.FormTemplate.Specifications;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.LanguageAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate.Create;

public class Endpoint(IRepository<FormTemplateAggregate> repository,
    IReadRepository<Language> languagesRepository,
    ITimeProvider timeProvider) :
        Endpoint<Request, Results<Ok<FormTemplateModel>, Conflict<ProblemDetails>>>
{

    public override void Configure()
    {
        Post("/api/form-templates");
    }

    public override async Task<Results<Ok<FormTemplateModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormTemplate(req.Code, req.FormType);
        var duplicatedFormTemplate = await repository.AnyAsync(specification, ct);

        if (duplicatedFormTemplate)
        {
            AddError(r => r.Code, "A form template with same parameters already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var languages = await languagesRepository.ListAsync(new LanguagesByIsoCode(req.Languages), ct);
        var formTemplate = FormTemplateAggregate.Create(req.FormType, req.Code, req.Name, languages, timeProvider);

        await repository.AddAsync(formTemplate, ct);

        return TypedResults.Ok(new FormTemplateModel
        {
            Id = formTemplate.Id,
            Name = formTemplate.Name,
            Status = formTemplate.Status,
            CreatedOn = formTemplate.CreatedOn,
            LastModifiedOn = formTemplate.LastModifiedOn,
            Sections = []
        });
    }
}
