using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository) : Endpoint<Request, Results<Ok<FormFullModel>, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgoId}/forms");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<Ok<FormFullModel>, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var existingFormsSpecification = new GetExistingFormsByCodeAndTypeSpecification(req.ElectionRoundId, req.MonitoringNgoId, req.Code, req.FormType);

        var isDuplicate = await formsRepository.AnyAsync(existingFormsSpecification, ct);
        if (isDuplicate)
        {
            AddError(x => x.Code, "A form with same code and form type already exists.");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var form = FormAggregate.Create(req.ElectionRoundId, req.MonitoringNgoId, req.FormType, req.Code, req.Name, req.DefaultLanguage, req.Languages, []);

        var monitoringNgo = await monitoringNgoRepository.GetByIdAsync(req.MonitoringNgoId, ct);
        monitoringNgo!.UpdatePollingStationsVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        await formsRepository.AddAsync(form, ct);
        return TypedResults.Ok(FormFullModel.FromEntity(form));
    }
}
