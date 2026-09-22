using Feature.Forms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Forms.FetchCitizenReportingForms;

public class Endpoint(VoteMonitorContext context, IFusionCache cache, IServiceScopeFactory scopeFactory)
    : Endpoint<Request, Results<NotFound, Ok<NgoFormsResponseModel>>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reporting-forms");

        DontAutoTag();
        Options(x => x.WithTags("citizen-reporting-forms", "forms", "public"));
        AllowAnonymous();
        Description(x => x.Accepts<Request>());
        Summary(s => { s.Summary = "Gets all published citizen reporting forms for an election round"; });
    }

    public override async Task<Results<NotFound, Ok<NgoFormsResponseModel>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var monitoringNgo = await context.ElectionRounds
            .Include(x => x.MonitoringNgoForCitizenReporting)
            .Where(x => x.Id == req.ElectionRoundId)
            .Where(x => x.MonitoringNgoForCitizenReporting != null)
            .Select(x => new
            {
                ElectionRoundId = x.Id,
                FormsVersion = x.MonitoringNgoForCitizenReporting!.FormsVersion,
                MonitoringNgoForCitizenReportingId = x.MonitoringNgoForCitizenReportingId
            })
            .FirstOrDefaultAsync(ct);

        if (monitoringNgo is null)
        {
            return TypedResults.NotFound();
        }

        var cacheKey =
            $"election-rounds/{req.ElectionRoundId}/monitoring-ngo/{monitoringNgo.MonitoringNgoForCitizenReportingId}/citizen-reports-forms/{monitoringNgo.FormsVersion}";

        var cachedResponse = await cache.GetOrSetAsync(
            cacheKey,
            async factoryCt =>
            {
                // Fresh scope: FusionCache eager refresh may outlive the request DbContext.
                await using var scope = scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();

                var forms = await db.Forms
                    .Where(x => x.Status == FormStatus.Published)
                    .Where(x => x.ElectionRoundId == req.ElectionRoundId)
                    .Where(x => x.MonitoringNgoId == monitoringNgo.MonitoringNgoForCitizenReportingId)
                    .Where(x => x.FormType == FormType.CitizenReporting)
                    .OrderBy(x => x.Code)
                    .ToListAsync(cancellationToken: factoryCt);

                return new NgoFormsResponseModel
                {
                    ElectionRoundId = monitoringNgo.ElectionRoundId,
                    Version = monitoringNgo.FormsVersion.ToString(),
                    Forms = forms.Select(FormFullModel.FromEntity).OrderBy(x => x.DisplayOrder).ToList()
                };
            },
            options => options.SetDuration(TimeSpan.FromMinutes(30)),
            token: ct);

        return TypedResults.Ok(cachedResponse);
    }
}
