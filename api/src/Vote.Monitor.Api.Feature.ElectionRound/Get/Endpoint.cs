using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.Feature.ElectionRound.Get;

public class Endpoint(
    VoteMonitorContext context,
    ICurrentUserProvider userProvider,
    ICurrentUserRoleProvider roleProvider)
    : Endpoint<Request, Results<Ok<ElectionRoundModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{id}");
        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task<Results<Ok<ElectionRoundModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        if (roleProvider.IsPlatformAdmin())
        {
            return await GetElectionRoundAsPlatformAdmin(req, ct);
        }

        return await GetElectionRoundAsNgoAdmin(req, ct);
    }

    private async Task<Results<Ok<ElectionRoundModel>, NotFound>> GetElectionRoundAsPlatformAdmin(Request req,
        CancellationToken ct)
    {
        var electionRound = await context.ElectionRounds.Where(x => x.Id == req.Id)
            .Include(x => x.MonitoringNgos)
            .ThenInclude(x => x.Ngo)
            .Include(x => x.MonitoringNgos)
            .ThenInclude(x => x.MonitoringObservers)
            .Include(x => x.Country)
            .AsSplitQuery()
            .Select(electionRound => new ElectionRoundModel
            {
                Id = electionRound.Id,
                CountryId = electionRound.CountryId,
                CountryIso2 = electionRound.Country.Iso2,
                CountryIso3 = electionRound.Country.Iso3,
                CountryName = electionRound.Country.Name,
                CountryFullName = electionRound.Country.FullName,
                CountryNumericCode = electionRound.Country.NumericCode,
                Title = electionRound.Title,
                EnglishTitle = electionRound.EnglishTitle,
                Status = electionRound.Status,
                StartDate = electionRound.StartDate,
                LastModifiedOn = electionRound.LastModifiedOn,
                CreatedOn = electionRound.CreatedOn,
                CoalitionId = null,
                CoalitionName = null,
                IsCoalitionLeader = false,
                IsMonitoringNgoForCitizenReporting = false,
            })
            .FirstOrDefaultAsync(ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(electionRound);
    }

    private async Task<Results<Ok<ElectionRoundModel>, NotFound>> GetElectionRoundAsNgoAdmin(Request req,
        CancellationToken ct)
    {
        var ngoId = userProvider.GetNgoId()!.Value;

        var electionRound = await context.MonitoringNgos
            .Include(x => x.ElectionRound)
            .ThenInclude(x => x.MonitoringNgoForCitizenReporting)
            .Where(x => x.NgoId == ngoId)
            .Where(x => x.ElectionRoundId == req.Id)
            .OrderBy(x => x.ElectionRound.StartDate)
            .Select(x => new ElectionRoundModel
            {
                Id = x.ElectionRound.Id,
                CountryId = x.ElectionRound.CountryId,
                CountryIso2 = x.ElectionRound.Country.Iso2,
                CountryIso3 = x.ElectionRound.Country.Iso3,
                CountryName = x.ElectionRound.Country.Name,
                CountryFullName = x.ElectionRound.Country.FullName,
                CountryNumericCode = x.ElectionRound.Country.NumericCode,
                Title = x.ElectionRound.Title,
                EnglishTitle = x.ElectionRound.EnglishTitle,
                Status = x.ElectionRound.Status,
                StartDate = x.ElectionRound.StartDate,
                LastModifiedOn = x.ElectionRound.LastModifiedOn,
                CreatedOn = x.ElectionRound.CreatedOn,
                IsMonitoringNgoForCitizenReporting = x.ElectionRound.CitizenReportingEnabled &&
                                                     x.ElectionRound.MonitoringNgoForCitizenReporting.NgoId ==
                                                     ngoId,
                IsCoalitionLeader =
                    context.Coalitions.Any(c => c.Leader.NgoId == ngoId && c.ElectionRoundId == x.ElectionRoundId),
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
            })
            .FirstOrDefaultAsync(ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(electionRound);
    }
}
