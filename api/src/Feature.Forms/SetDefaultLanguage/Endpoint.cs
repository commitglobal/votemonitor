using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.SetDefaultLanguage;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/forms/{id}:setDefaultLanguage");
        Policies(PolicyNames.NgoAdminsOnly);
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
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

        if (!form.HasTranslation(req.LanguageCode))
        {
            AddError(x => x.LanguageCode, "Form does not have translations for language code");
        }
        form.SetDefaultLanguage(req.LanguageCode);

        await formsRepository.UpdateAsync(form, ct);

        var monitoringNgo = await monitoringNgoRepository.FirstOrDefaultAsync(new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        monitoringNgo!.UpdatePollingStationsVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }
}
