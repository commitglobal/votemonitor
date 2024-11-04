using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.Feature.ElectionRound.ListAvailableForCitizenReporting;

public class Endpoint(VoteMonitorContext context) : EndpointWithoutRequest<Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds:citizen-report");
        DontAutoTag();
        Options(x => x.WithTags("election-rounds", "citizen-reports", "public"));
        AllowAnonymous();
        
        Summary(s =>
        {
            s.Summary = "Lists election rounds which can be monitored by citizens";
        });
    }

    public override async Task<Response> ExecuteAsync(CancellationToken ct)
    {
        var electionRounds = await context.ElectionRounds
            .Include(x => x.Country)
            .Where(x => x.CitizenReportingEnabled && x.Status == ElectionRoundStatus.Started)
            .Select(x => new ElectionRoundModel
            {
                Id = x.Id,
                CountryCode = x.Country.Iso2,
                CountryName = x.Country.Name,
                CountryFullName = x.Country.FullName,
                StartDate = x.StartDate,
                Title = x.Title,
            })
            .ToListAsync(ct);
        
        return new Response
        {
            ElectionRounds = electionRounds
        };
    }
}