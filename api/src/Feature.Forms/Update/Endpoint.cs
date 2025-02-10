using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Form.Module.Mappers;

namespace Feature.Forms.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormByIdSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var form = await formsRepository.FirstOrDefaultAsync(specification, ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        if (form.Status == FormStatus.Published)
        {
            ThrowError(x => x.Id, "Cannot edit published form");
        }

        var questions = req.Questions
            .Select(QuestionsMapper.ToEntity)
            .ToList()
            .AsReadOnly();

        form.UpdateDetails(req.Code, req.Name, req.Description, req.FormType, req.DefaultLanguage, req.Languages,
            req.Icon, displayOrder: 0, questions);

        await formsRepository.UpdateAsync(form, ct);

        var monitoringNgo =
            await monitoringNgoRepository.FirstOrDefaultAsync(
                new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        monitoringNgo!.UpdateFormVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }
}
