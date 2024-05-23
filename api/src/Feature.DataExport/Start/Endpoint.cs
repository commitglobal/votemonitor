using Authorization.Policies;
using Authorization.Policies.Requirements;
using Job.Contracts;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Feature.DataExport.Start;

public class Endpoint(IJobService jobService,
    IAuthorizationService authorizationService,
    IRepository<ExportedData> repository,
    ITimeProvider timeProvider) : Endpoint<Request, Results<Ok<JobDetails>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/exported-data");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("exported-data"));

        Summary(s =>
        {
            s.Summary = "Enqueues a job to export data and returns job id to poll for results";
        });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<JobDetails>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!result.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var exportedData = ExportedData.Create(req.ElectionRoundId, req.NgoId, req.ExportedDataType, timeProvider.UtcNow);

        await repository.AddAsync(exportedData, ct);

        if (req.ExportedDataType == ExportedDataType.FormSubmissions)
        {
            jobService.ExportFormSubmissions(req.ElectionRoundId, req.NgoId, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.QuickReports)
        {
            jobService.ExportQuickReportsSubmissions(req.ElectionRoundId, req.NgoId, exportedData.Id);
        }

        return TypedResults.Ok(new JobDetails
        {
            ExportedDataId = exportedData.Id,
            EnqueuedAt = timeProvider.UtcNow
        });
    }
}
