using Feature.Forms.Models;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.Forms.GetFormsVersion;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Results<Ok<FormVersionResponseModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms:version");
        DontAutoTag();
        Options(x => x.WithTags("forms", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets current version of forms for users monitoring ngo";
            s.Description = "Cache key changes every time any form changes";
        });
    }

    public override async Task<Results<Ok<FormVersionResponseModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var monitoringNgo = await context.MonitoringObservers
            .Include(x => x.MonitoringNgo)
            .Where(x => x.ObserverId == req.ObserverId)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId)
            .Where(x => x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .Select(x => new { x.MonitoringNgo.FormsVersion, x.MonitoringNgo.ElectionRoundId })
            .FirstOrDefaultAsync(ct);

        if (monitoringNgo is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new FormVersionResponseModel
        {
            ElectionRoundId = monitoringNgo.ElectionRoundId, CacheKey = monitoringNgo.FormsVersion.ToString()
        });
    }
}
