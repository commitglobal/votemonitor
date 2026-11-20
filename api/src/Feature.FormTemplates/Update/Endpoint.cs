using Feature.FormTemplates.Specifications;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.Update;

public class Endpoint(IRepository<FormTemplate> repository, IUserDisplayNameProvider userDisplayNames)
    : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>, Conflict<VersionConflict<FormTemplateFullModel>>>>
{
    public override void Configure()
    {
        Put("/api/form-templates/{id}");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>, Conflict<VersionConflict<FormTemplateFullModel>>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        if (RecordVersions.IsStale(req.Version, formTemplate.Version))
        {
            return await ConflictAsync(req.Id, ct);
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

        try
        {
            await repository.UpdateAsync(formTemplate, ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Saved by someone else between reading and writing it.
            return await ConflictAsync(req.Id, ct);
        }

        return TypedResults.NoContent();
    }

    private async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>, Conflict<VersionConflict<FormTemplateFullModel>>>> ConflictAsync(Guid id,
        CancellationToken ct)
    {
        var current = await repository.FirstOrDefaultAsync(new GetFormTemplateReadOnlySpecification(id, false), ct);
        return current is null
            ? TypedResults.NotFound()
            : TypedResults.Conflict(
                VersionConflict<FormTemplateFullModel>.Of(await FormTemplateFullModel.FromEntityAsync(current, userDisplayNames, ct)));
    }
}
