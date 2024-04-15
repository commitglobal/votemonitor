using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;

namespace Feature.MonitoringObservers.List;
public class Endpoint(IAuthorizationService authorizationService,
    IReadRepository<MonitoringObserverAggregate> repository) : Endpoint<Request, Results<Ok<PagedResponse<MonitoringObserverModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers");
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Lists monitoring observers";
        });
    }

    public override async Task<Results<Ok<PagedResponse<MonitoringObserverModel>>, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(request.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var listSpecification = new ListMonitoringObserverSpecification(request);
        var monitoringObservers = await repository.ListAsync(listSpecification, ct);
        var count = await repository.CountAsync(listSpecification, ct);

        var pagedResponse = new PagedResponse<MonitoringObserverModel>(monitoringObservers, count, request.PageNumber, request.PageSize);
        return TypedResults.Ok(pagedResponse);
    }
}
