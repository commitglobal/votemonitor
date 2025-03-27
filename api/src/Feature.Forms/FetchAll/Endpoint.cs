using Feature.Forms.Models;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.Forms.FetchAll;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<NgoFormsResponseModel>, NotFound>>
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

    public override async Task<Results<Ok<NgoFormsResponseModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var monitoringNgo = await context.MonitoringObservers
            .Include(x => x.MonitoringNgo)
            .ThenInclude(x => x.Memberships.Where(m => m.ElectionRoundId == req.ElectionRoundId))
            .ThenInclude(cm => cm.Coalition)
            .Where(x => x.ObserverId == req.ObserverId)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId)
            .Where(x => x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .Select(x => new
            {
                x.MonitoringNgo.ElectionRoundId,
                x.MonitoringNgoId,
                x.MonitoringNgo.FormsVersion,
                IsCoalitionLeader =
                    x.MonitoringNgo.Memberships.Any(m =>
                        m.ElectionRoundId == req.ElectionRoundId && m.Coalition.LeaderId == x.MonitoringNgoId),
                IsInACoalition = x.MonitoringNgo.Memberships.Count != 0
            })
            .FirstOrDefaultAsync(ct);

        if (monitoringNgo is null)
        {
            return TypedResults.NotFound();
        }

        List<FormFullModel> resultForms = new List<FormFullModel>();
        if (monitoringNgo.IsInACoalition || monitoringNgo.IsCoalitionLeader)
        {
            resultForms.AddRange(await context.CoalitionFormAccess
                .Include(x => x.Form)
                .Where(x => x.MonitoringNgoId == monitoringNgo.MonitoringNgoId &&
                            x.Coalition.ElectionRoundId == req.ElectionRoundId)
                .Where(x => x.Form.FormType != FormType.CitizenReporting)
                .Select(f => FormFullModel.FromEntity(f.Form))
                .AsNoTracking()
                .ToListAsync(ct));
        }

        if (!monitoringNgo.IsCoalitionLeader)
        {
            resultForms.AddRange(await context.Forms
                .Where(x => x.Status == FormStatus.Published)
                .Where(x => x.ElectionRoundId == req.ElectionRoundId)
                .Where(x => x.MonitoringNgoId == monitoringNgo.MonitoringNgoId)
                .Where(x => x.FormType != FormType.CitizenReporting)
                .Select(f => FormFullModel.FromEntity(f))
                .AsNoTracking()
                .ToListAsync(cancellationToken: ct));
        }

        return TypedResults.Ok(new NgoFormsResponseModel
        {
            ElectionRoundId = monitoringNgo.ElectionRoundId,
            Version = DeterministicGuid.Create(resultForms.Select(x => x.Id)).ToString(),
            Forms = resultForms.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Code).ToList()
        });
    }
}
