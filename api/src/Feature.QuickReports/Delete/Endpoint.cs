using Authorization.Policies.Requirements;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Delete;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<QuickReport> repository,
    VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/quick-reports/{id}");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports", "mobile"));
        Summary(s =>
        {
            s.Summary = "Deletes a quick report and it's attachments";
        });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        await DeleteQuickReportAsync(req, ct);
        await DeleteAttachmentsAsync(req, ct);

        return TypedResults.NoContent();
    }

    private async Task DeleteQuickReportAsync(Request req, CancellationToken ct)
    {
        var quickReport = await repository.FirstOrDefaultAsync(new GetQuickReportByIdSpecification(req.ElectionRoundId, req.Id), ct);
        if (quickReport is null)
        {
            return;
        }

        await repository.DeleteAsync(quickReport, ct);
    }

    private async Task DeleteAttachmentsAsync(Request req, CancellationToken ct)
    {
        // Mark as deleted so our background job will delete it. also change the id in order to allow recreation with same id
        await context.QuickReportAttachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.ObserverId == req.ObserverId
                        && x.QuickReportId == req.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(qra => qra.IsDeleted, true)
                .SetProperty(qra => qra.Id, Guid.NewGuid()), cancellationToken: ct);
    }
}
