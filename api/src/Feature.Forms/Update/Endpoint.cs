using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Get;
using Feature.Forms.Models;
using Feature.Forms.Specifications;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Module.Forms.Mappers;

namespace Feature.Forms.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<NoContent, NotFound, Conflict<VersionConflict<FormFullModel>>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, Conflict<VersionConflict<FormFullModel>>>> ExecuteAsync(
        Request req, CancellationToken ct)
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

        if (RecordVersions.IsStale(req.Version, form.Version))
        {
            return await ConflictAsync(req, ct);
        }

        var questions = req.Questions
            .Select(QuestionsMapper.ToEntity)
            .ToList()
            .AsReadOnly();

        form.UpdateDetails(req.Code, req.Name, req.Description, req.FormType, req.DefaultLanguage, req.Languages,
            req.Icon, questions);

        try
        {
            await formsRepository.UpdateAsync(form, ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Saved by someone else between reading and writing it.
            return await ConflictAsync(req, ct);
        }

        var monitoringNgo =
            await monitoringNgoRepository.FirstOrDefaultAsync(
                new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        monitoringNgo!.UpdateFormVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.NoContent();
    }

    private async Task<Results<NoContent, NotFound, Conflict<VersionConflict<FormFullModel>>>> ConflictAsync(Request req,
        CancellationToken ct)
    {
        var current = await FormDetailsQuery.GetAsync(dbConnectionFactory, req.ElectionRoundId, req.NgoId, req.Id, ct);
        return current is null
            ? TypedResults.NotFound()
            : TypedResults.Conflict(VersionConflict<FormFullModel>.Of(current));
    }
}
