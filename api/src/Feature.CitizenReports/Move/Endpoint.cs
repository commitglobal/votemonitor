using Feature.CitizenReports.Specifications;
using Vote.Monitor.Domain.Entities.LocationAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.CitizenReports.Move;

public class Endpoint(
    IRepository<CitizenReportAggregate> repository,
    IReadRepository<Location> locationRepository,
    IRecordAutomation recordAutomation)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}:move");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s => { s.Summary = "Moves a citizen report to another location (platform admins)"; });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var citizenReport = await repository.FirstOrDefaultAsync(
            new GetCitizenReportByIdForUpdateSpecification(req.ElectionRoundId, req.CitizenReportId), ct);
        if (citizenReport is null)
        {
            return TypedResults.NotFound();
        }

        var location = await locationRepository.FirstOrDefaultAsync(
            new GetLocationSpecification(req.ElectionRoundId, req.LocationId), ct);
        if (location is null)
        {
            return TypedResults.NotFound();
        }

        citizenReport.MoveToLocation(location);
        await repository.UpdateAsync(citizenReport, ct);
        await recordAutomation.RecordEditedAsync(citizenReport.ElectionRoundId, ReportRecordType.CitizenReport,
            citizenReport.Id, ct);

        return TypedResults.NoContent();
    }
}
