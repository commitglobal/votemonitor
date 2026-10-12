using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.PollingStation.Information.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.GetById;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider userRoleProvider,
    IReadRepository<PollingStationInformation> repository)
    : Endpoint<Request, Results<Ok<PollingStationInformationModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/information/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
        Summary(s => { s.Summary = "Gets polling station information by id"; });
        Policies(PolicyNames.NotObservers);
    }

    public override async Task<Results<Ok<PollingStationInformationModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var isPlatformAdmin = userRoleProvider.IsPlatformAdmin();

        if (!isPlatformAdmin)
        {
            var authorizationResult =
                await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
            if (!authorizationResult.Succeeded || !req.NgoId.HasValue || req.NgoId == Guid.Empty)
            {
                return TypedResults.NotFound();
            }
        }

        var specification = new GetPollingStationInformationByIdSpecification(req.ElectionRoundId, req.Id,
            isPlatformAdmin ? null : req.NgoId);
        var pollingStationInformation = await repository.FirstOrDefaultAsync(specification, ct);

        return pollingStationInformation is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(pollingStationInformation);
    }
}
