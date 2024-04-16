using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Form.Module.Mappers;

namespace Feature.Forms.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository) : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgoId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormByIdSpecification(req.ElectionRoundId, req.MonitoringNgoId, req.Id);
        var form = await formsRepository.FirstOrDefaultAsync(specification, ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        var exitingFormsSpecification = new GetExistingFormsByCodeAndTypeSpecification(req.ElectionRoundId, req.MonitoringNgoId, req.Id, req.Code, req.FormType);
        var duplicatedFormTemplate = await formsRepository.AnyAsync(exitingFormsSpecification, ct);

        if (duplicatedFormTemplate)
        {
            AddError(r => r.Name, "A form template with same parameters already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var questions = req.Questions
                 .Select(QuestionsMapper.ToEntity)
                 .ToList()
                 .AsReadOnly();

        form.UpdateDetails(req.Code, req.Name, req.Description, req.FormType, req.DefaultLanguage, req.Languages, questions);

        await formsRepository.UpdateAsync(form, ct);

        var monitoringNgo = await monitoringNgoRepository.GetByIdAsync(req.MonitoringNgoId, ct);
        monitoringNgo!.UpdatePollingStationsVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }
}
