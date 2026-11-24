using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Models;
using Vote.Monitor.Domain;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.FromTemplate;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<FormTemplate> formTemplateRepository,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository,
    IUserDisplayNameProvider userDisplayNames) : Endpoint<Request, Results<Ok<FormFullModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/forms:fromTemplate");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<FormFullModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var template = await formTemplateRepository.GetByIdAsync(req.TemplateId, ct);
        if (template == null)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgo =
            await monitoringNgoRepository.FirstOrDefaultAsync(
                new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        
        var form = template.Clone(req.ElectionRoundId, monitoringNgo!.Id, req.DefaultLanguage, req.Languages);
        await formsRepository.AddAsync(form, ct);

        monitoringNgo!.UpdateFormVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        var model = FormFullModel.FromEntity(form);
        model.LastModifiedByName = await userDisplayNames.GetDisplayNameAsync(form.LastModifiedBy, ct);
        return TypedResults.Ok(model);
    }
}
