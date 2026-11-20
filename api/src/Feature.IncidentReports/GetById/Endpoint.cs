using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.RecordLabelAggregate;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;
using Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;

namespace Feature.IncidentReports.GetById;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider userRoleProvider,
    VoteMonitorContext context,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NotObservers);

        Summary(s =>
        {
            s.Summary = "Gets incident report by id including notes and attachments";
            s.Description = "Platform admins get who sent it, from where and its corrections, never what was reported";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var isPlatformAdmin = userRoleProvider.IsPlatformAdmin();
        // Platform admins read any report of the round; ForPlatformAdmin then leaves out what they mustn't see.
        Guid? ngoId = null;
        if (!isPlatformAdmin)
        {
            var authorizationResult =
                await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
            if (!authorizationResult.Succeeded || req.NgoId is not { } callerNgoId || callerNgoId == Guid.Empty)
            {
                return TypedResults.NotFound();
            }

            ngoId = callerNgoId;
        }

        var incidentReport = await context
            .IncidentReports
            .AsNoTracking()
            .Include(x => x.Form)
            .Include(x => x.Attachments)
            .Include(x => x.Notes)
            .Include(x => x.PollingStation)
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.Form.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                && (ngoId == null || x.Form.MonitoringNgo.NgoId == ngoId)
                && x.Id == req.IncidentReportId)
            .Select(incidentReport => new
            {
                Id = incidentReport.Id,
                FormId = incidentReport.Form.Id,
                FormCode = incidentReport.Form.Code,
                FormName = incidentReport.Form.Name,
                FormDefaultLanguage = incidentReport.Form.DefaultLanguage,
                FormQuestions = incidentReport.Form.Questions,
                Answers = incidentReport.Answers,
                Notes = incidentReport.Notes,
                Attachments = incidentReport.Attachments,
                Questions = incidentReport.Form.Questions,
                MonitoringObserverId = incidentReport.MonitoringObserverId,
                ObserverName = incidentReport.MonitoringObserver.Observer.ApplicationUser.DisplayName,
                Email = incidentReport.MonitoringObserver.Observer.ApplicationUser.Email,
                NgoName = incidentReport.MonitoringObserver.MonitoringNgo.Ngo.Name,
                TimeSubmitted = incidentReport.LastUpdatedAt,
                FollowUpStatus = incidentReport.FollowUpStatus,
                LocationType = incidentReport.LocationType,
                LocationDescription = incidentReport.LocationDescription,
                PollingStation = incidentReport.PollingStation,
                IsCompleted = incidentReport.IsCompleted,
                NumberOfQuestionsAnswered = incidentReport.NumberOfQuestionsAnswered,
                NumberOfUnansweredQuestions = incidentReport.NumberOfUnansweredQuestions
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (incidentReport == null)
        {
            return TypedResults.NotFound();
        }

        var pollingStationCorrections = await context.GetPollingStationCorrectionsAsync(req.ElectionRoundId,
            CorrectableRecordType.IncidentReport, incidentReport.Id, ct);
        var basics = new Response
        {
            IncidentReportId = incidentReport.Id,
            FormId = incidentReport.FormId,
            FormCode = incidentReport.FormCode,
            FormName = incidentReport.FormName,
            FormDefaultLanguage = incidentReport.FormDefaultLanguage,
            MonitoringObserverId = incidentReport.MonitoringObserverId,
            ObserverName = incidentReport.ObserverName,
            Email = incidentReport.Email!,
            NgoName = incidentReport.NgoName,
            TimeSubmitted = incidentReport.TimeSubmitted,
            LocationType = incidentReport.LocationType,
            PollingStationId = incidentReport.PollingStation?.Id,
            PollingStationLevel1 = incidentReport.PollingStation?.Level1,
            PollingStationLevel2 = incidentReport.PollingStation?.Level2,
            PollingStationLevel3 = incidentReport.PollingStation?.Level3,
            PollingStationLevel4 = incidentReport.PollingStation?.Level4,
            PollingStationLevel5 = incidentReport.PollingStation?.Level5,
            PollingStationNumber = incidentReport.PollingStation?.Number,
            PollingStationAddress = incidentReport.PollingStation?.Address,
            IsCompleted = incidentReport.IsCompleted,
            PollingStationCorrections = pollingStationCorrections
        };
        if (isPlatformAdmin)
        {
            return TypedResults.Ok(basics.ForPlatformAdmin());
        }

        var attachments = await fileStorageService.PresignAsync(
            incidentReport.Attachments.Select(AttachmentMapper.ToModel));
        var followUp = await context.GetFollowUpStatusAsync(req.ElectionRoundId, ngoId!.Value, ReportRecordType.IncidentReport,
            incidentReport.Id, ct);
        var reviewTarget = await context.GetReviewTargetAsync(req.ElectionRoundId, ngoId.Value, ReportRecordType.IncidentReport, incidentReport.Id, ct);
        var reviewWorkflow = reviewTarget is null ? null : await context.GetReviewWorkflowAsync(reviewTarget, ct);
        var recordSeverity = reviewTarget is null ? null : await context.GetRecordSeverityAsync(reviewTarget, ct);
        var recordLabels = reviewTarget is null ? null : await context.GetRecordLabelsAsync(reviewTarget, ct);
        var response = basics with
        {
            FollowUpStatusId = followUp?.Id,
            FollowUpStatusName = followUp?.Name,
            FollowUpStatusColor = followUp?.Color,
            FollowUpStatusIcon = followUp?.Icon,
            ReviewStage = reviewWorkflow?.Stage,
            StageChangedAt = reviewWorkflow?.StageChangedAt,
            ReviewedAt = reviewWorkflow?.ReviewedAt,
            Reviewers = reviewWorkflow?.Reviewers,
            Severity = recordSeverity?.Severity,
            SeverityChangedAt = recordSeverity?.ChangedAt,
            Labels = recordLabels,
            Answers = incidentReport.Answers.Select(AnswerMapper.ToModel).ToArray(),
            Notes = incidentReport.Notes.Select(NoteModel.FromEntity).ToArray(),
            Attachments = attachments,
            Questions = incidentReport.FormQuestions.Select(QuestionsMapper.ToModel).ToArray(),
            FollowUpStatus = incidentReport.FollowUpStatus,
            LocationDescription = incidentReport.LocationDescription,
            NumberOfQuestionsAnswered = incidentReport.NumberOfQuestionsAnswered,
            NumberOfUnansweredQuestions = incidentReport.NumberOfUnansweredQuestions
        };

        return TypedResults.Ok(response);
    }
}
