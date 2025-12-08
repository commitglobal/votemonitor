using Authorization.Policies.Requirements;
using Feature.Forms.Models;
using Feature.Forms.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;
using GetCoalitionFormSpecification = Feature.Forms.Specifications.GetCoalitionFormSpecification;

namespace Feature.Forms.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<FormAggregate> formRepository,
    IReadRepository<Coalition> coalitionRepository,
    IReadRepository<PollingStationInformationForm> psiFormRepository) : Endpoint<Request, Results<Ok<FormFullModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<Ok<FormFullModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        var psiFormSpecification =                                               
            new GetPsiFormById(req.ElectionRoundId, req.Id); 
        var coalitionFormSpecification =
            new GetCoalitionFormSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var ngoFormSpecification =
            new GetFormByIdSpecification(req.ElectionRoundId, req.NgoId, req.Id);

        var psiForm = await psiFormRepository.FirstOrDefaultAsync(psiFormSpecification, ct);

        if (psiForm is not null)
        {
            return TypedResults.Ok(FormFullModel.FromEntity(psiForm));
        }
        
        var form = (await coalitionRepository.FirstOrDefaultAsync(coalitionFormSpecification, ct)) ??
                   (await formRepository.FirstOrDefaultAsync(ngoFormSpecification, ct));

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(FormFullModel.FromEntity(form));
    }
}
