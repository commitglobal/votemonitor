using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.Feature.ElectionRound.Monitoring;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Ok<Result>>
{
    public override void Configure()
    {
        Get("/api/election-rounds:monitoring");
        DontAutoTag();
        Options(x => x.WithTags("election-rounds"));
        Summary(s =>
        {
            s.Summary = "Lists election rounds which are monitored by current NGO";
            s.Description = "Election rounds with status NotStarted and Started are listed";
        });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Ok<Result>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRounds = await context.MonitoringNgos
            .Include(x => x.ElectionRound)
            .ThenInclude(x => x.MonitoringNgoForCitizenReporting)
            .Where(x => x.NgoId == req.NgoId)
            .OrderBy(x => x.ElectionRound.StartDate)
            .Select(x => new NgoElectionRoundView
            {
                MonitoringNgoId = x.Id,
                ElectionRoundId = x.ElectionRoundId,
                Title = x.ElectionRound.Title,
                EnglishTitle = x.ElectionRound.EnglishTitle,
                StartDate = x.ElectionRound.StartDate,
                Country = x.ElectionRound.Country.FullName,
                CountryId = x.ElectionRound.CountryId,
                IsMonitoringNgoForCitizenReporting = x.ElectionRound.CitizenReportingEnabled &&
                                                     x.ElectionRound.MonitoringNgoForCitizenReporting.NgoId ==
                                                     req.NgoId,
                IsCoalitionLeader =
                    context.Coalitions.Any(c => c.Leader.NgoId == req.NgoId && c.ElectionRoundId == x.ElectionRoundId),
                Status = x.ElectionRound.Status,
                CoalitionName = context.Coalitions
                    .Where(c =>
                        c.Memberships.Any(m => m.MonitoringNgoId == x.Id) && c.ElectionRoundId == x.ElectionRoundId)
                    .Select(c => c.Name)
                    .FirstOrDefault(),
                CoalitionId = context.Coalitions
                    .Where(c =>
                        c.Memberships.Any(m => m.MonitoringNgoId == x.Id) && c.ElectionRoundId == x.ElectionRoundId)
                    .Select(c => c.Id)
                    .FirstOrDefault()
            }).ToListAsync(ct);

        return TypedResults.Ok(new Result { ElectionRounds = electionRounds });
    }
}
