using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.FetchAll;
public class Endpoint(VoteMonitorContext context, IMemoryCache cache) : Endpoint<Request, Results<Ok<Response>, NotFound>>
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

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var monitoringNgo = await context.MonitoringNgos
            .Where(x => x.ElectionRoundId == request.ElectionRoundId)
            .Select(x => new { x.FormsVersion, x.Id })
            .FirstOrDefaultAsync(ct);

        if (monitoringNgo is null)
        {
            return TypedResults.NotFound();
        }

        var cacheKey = $"election-rounds/{request.ElectionRoundId}/monitoring-ngo/{monitoringNgo.Id}/forms/{monitoringNgo.FormsVersion}";

        var cachedResponse = await cache.GetOrCreateAsync(cacheKey, async (e) =>
        {
            var forms = await context.Forms
                .Where(x=>x.Status == FormStatus.Published)
                .Where(x => x.ElectionRoundId == request.ElectionRoundId)
                .Where(x => x.MonitoringNgoId == monitoringNgo.Id)
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken: ct);

            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

            return new Response
            {
                ElectionRoundId = monitoringNgo.Id,
                Version = monitoringNgo.FormsVersion.ToString(),
                Forms = forms.Select(FormFullModel.FromEntity).ToList()
            };
        });

        return TypedResults.Ok(cachedResponse!);
    }
}
