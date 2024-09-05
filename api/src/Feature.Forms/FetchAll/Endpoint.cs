using Feature.Forms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.FetchAll;
public class Endpoint(VoteMonitorContext context, IMemoryCache cache) : Endpoint<Request, Results<Ok<NgoFormsResponseModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms:fetchAll");
        DontAutoTag();
        Options(x => x.WithTags("forms", "mobile"));
        Description(x => x.Accepts<Request>());
        Summary(s =>
        {
            s.Summary = "Gets all published forms by an ngo for an election round";
            s.Description = "Gets all forms and a cache key for the data";
        });
    }

    public override async Task<Results<Ok<NgoFormsResponseModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var monitoringNgo = await context.MonitoringObservers
            .Include(x => x.MonitoringNgo)
            .Where(x => x.ObserverId == req.ObserverId)
            .Where(x => x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .Select(x => new
            {
                x.MonitoringNgo.ElectionRoundId,
                x.MonitoringNgoId,
                x.MonitoringNgo.FormsVersion
            })
            .FirstOrDefaultAsync(ct);

        if (monitoringNgo is null)
        {
            return TypedResults.NotFound();
        }

        var cacheKey = $"election-rounds/{req.ElectionRoundId}/monitoring-ngo/{monitoringNgo.MonitoringNgoId}/forms/{monitoringNgo.FormsVersion}";

        var cachedResponse = await cache.GetOrCreateAsync(cacheKey, async (e) =>
        {
            var forms = await context.Forms
                .Where(x => x.Status == FormStatus.Published)
                .Where(x => x.ElectionRoundId == req.ElectionRoundId)
                .Where(x => x.MonitoringNgoId == monitoringNgo.MonitoringNgoId)
                .Where(x=>x.FormType != FormType.CitizenReporting)
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken: ct);

            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

            return new NgoFormsResponseModel
            {
                ElectionRoundId = monitoringNgo.ElectionRoundId,
                Version = monitoringNgo.FormsVersion.ToString(),
                Forms = forms.Select(FormFullModel.FromEntity).ToList()
            };
        });

        return TypedResults.Ok(cachedResponse!);
    }
}
