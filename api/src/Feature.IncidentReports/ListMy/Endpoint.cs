namespace Feature.IncidentReports.ListMy;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports:my");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports", "mobile"));
        Summary(s => { s.Summary = "Gets all incident reports for an observer including notes and attachments"; });

        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        
        var incidentReports = await context
            .IncidentReports
            .Include(x => x.Attachments)
            .Include(x => x.Notes)
            .Include(x => x.PollingStation)
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId
                && x.MonitoringObserver.ObserverId == req.ObserverId)
            .Select(incidentReport => new
            {
                Id = incidentReport.Id,
                FormId = incidentReport.FormId,
                FormDefaultLanguage = incidentReport.Form.DefaultLanguage,
                FormQuestions = incidentReport.Form.Questions,
                Answers = incidentReport.Answers,
                Notes = incidentReport.Notes,
                Attachments = incidentReport.Attachments,
                Questions = incidentReport.Form.Questions,
                MonitoringObserverId = incidentReport.MonitoringObserverId,
                ObserverName = incidentReport.MonitoringObserver.Observer.ApplicationUser.LastName + " " +
                               incidentReport.MonitoringObserver.Observer.ApplicationUser.LastName,
                Timestamp = incidentReport.LastModifiedOn ?? incidentReport.CreatedOn,
                FollowUpStatus = incidentReport.FollowUpStatus,

                LocationType = incidentReport.LocationType,
                LocationDescription = incidentReport.LocationDescription,

                PollingStation = incidentReport.PollingStation
            })
            .AsSplitQuery()
            .ToListAsync(ct);
        var incidentReportsModels = new List<IncidentReportModel>();


        foreach (var incidentReport in incidentReports)
        {
            var tasks = incidentReport.Attachments
                .Select(AttachmentModel.FromEntity)
                .Select(async attachment =>
                {
                    var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                        attachment.FilePath,
                        attachment.UploadedFileName);

                    return attachment with
                    {
                        PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                        UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0
                    };
                }).ToArray();

            var attachments = await Task.WhenAll(tasks);

            incidentReportsModels.Add(new IncidentReportModel
            {
                Id = incidentReport.Id,
                FormId = incidentReport.FormId,
                Answers = incidentReport.Answers.Select(AnswerMapper.ToModel).ToArray(),
                Notes = incidentReport.Notes.Select(NoteModel.FromEntity).ToArray(),
                Attachments = attachments,
                FollowUpStatus = incidentReport.FollowUpStatus,
                LocationType = incidentReport.LocationType,
                LocationDescription = incidentReport.LocationDescription,
                PollingStationId = incidentReport.PollingStation?.Id,
                Timestamp = incidentReport.Timestamp
            });
        }

        var response = new Response
        {
            IncidentReports = incidentReportsModels
        };

        return TypedResults.Ok(response);
    }
}
