﻿using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.Forms.Models;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.FromForm;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<MonitoringNgo> monitoringNgoRepository,
    IRepository<FormAggregate> formsRepository) : Endpoint<Request, Results<Ok<FormFullModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/forms:fromForm");
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

        var form = await formsRepository.FirstOrDefaultAsync(new GetFormByIdSpecification(req.FormElectionRoundId, req.NgoId, req.FormId), ct);
        if (form == null)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgo =
            await monitoringNgoRepository.FirstOrDefaultAsync(
                new GetMonitoringNgoSpecification(req.ElectionRoundId, req.NgoId), ct);
        
        var newForm = form.Clone(req.ElectionRoundId, monitoringNgo!.Id, req.DefaultLanguage, req.Languages);
        await formsRepository.AddAsync(newForm, ct);

        monitoringNgo!.UpdateFormVersion();
        await monitoringNgoRepository.UpdateAsync(monitoringNgo, ct);

        return TypedResults.Ok(FormFullModel.FromEntity(newForm));
    }
}
