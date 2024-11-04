using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.Delete;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository,
    IRepository<FormSubmission> formSubmissionsRepository,
    IRepository<IncidentReport> incidentReportRepository) : Endpoint<Request, Results<NoContent, NotFound, Conflict>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, Conflict>> ExecuteAsync(Request req, CancellationToken ct)
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

        bool hasFormSubmissions = false;
        if (form.FormType == FormType.IncidentReporting)
        {
            hasFormSubmissions = await incidentReportRepository
                .AnyAsync(new GetIncidentReportsForFormSpecification(req.ElectionRoundId, req.NgoId, req.Id), ct);
        }
        else
        {
            hasFormSubmissions = await formSubmissionsRepository
                .AnyAsync(new GetSubmissionsForFormSpecification(req.ElectionRoundId, req.NgoId, req.Id), ct);
        }
        
        if (form.Status == FormStatus.Published && hasFormSubmissions)
        {
            return TypedResults.Conflict();
        }

        await formsRepository.DeleteAsync(form, ct);

        var monitoringNgo = await monitoringNgoRepository.FirstOrDefaultAsync(new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        monitoringNgo!.UpdateFormVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }
}
