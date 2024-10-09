using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Form.Module.Mappers;

namespace Feature.Forms.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository) : Endpoint<Request, Results<Ok<FormFullModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/forms");
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

        var monitoringNgo =
            await monitoringNgoRepository.FirstOrDefaultAsync(
                new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        monitoringNgo!.UpdateFormVersion();

        var questions = req.Questions.Select(QuestionsMapper.ToEntity)
            .ToList()
            .AsReadOnly();

        var form = FormAggregate.Create(req.ElectionRoundId, monitoringNgo.Id, req.FormType, req.Code, req.Name,
            req.Description, req.DefaultLanguage, req.Languages, questions);

        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        await formsRepository.AddAsync(form, ct);
        return TypedResults.Ok(FormFullModel.FromEntity(form));
    }
}