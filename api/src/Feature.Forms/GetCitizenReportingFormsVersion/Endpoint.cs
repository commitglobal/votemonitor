using Feature.Forms.Models;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.Forms.GetCitizenReportingFormsVersion;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Results<Ok<FormVersionResponseModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reporting-forms:version");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports", "public"));
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Gets current version of forms for citizen reporting";
            s.Description = "Cache key changes every time any form changes";
        });
    }

    public override async Task<Results<Ok<FormVersionResponseModel>, NotFound>> ExecuteAsync(Request req,
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

        return TypedResults.Ok(new FormVersionResponseModel
        {
            ElectionRoundId = monitoringNgo.ElectionRoundId,
            CacheKey = monitoringNgo.FormsVersion.ToString()
        });
    }
}