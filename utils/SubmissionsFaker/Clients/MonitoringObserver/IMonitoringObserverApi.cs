using Refit;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Clients.MonitoringObserver;

public interface IMonitoringObserverApi
{
    [Post("/api/election-rounds/{electionRoundId}/form-submissions")]
    Task SubmitForm(
        [AliasAs("electionRoundId")] string electionRoundId,
        [Body] SubmissionRequest submission,
        [Authorize] string token);

    [Multipart]
    [Post("/api/election-rounds/{electionRoundId}/attachments")]
    Task<IApiResponse> SubmitAttachment(
        [AliasAs("electionRoundId")] string electionRoundId,
        [AliasAs("PollingStationId")] string pollingStationId,
        [AliasAs("Id")] string id,
        [AliasAs("FormId")] string formId,
        [AliasAs("QuestionId")] string questionId,
        [AliasAs("Attachment")] StreamPart attachment,
        [Authorize] string token);

    [Post("/api/election-rounds/{electionRoundId}/notes")]
    Task SubmitNote(
        [AliasAs("electionRoundId")] string electionRoundId,
        [Body] NoteRequest note,
        [Authorize] string token);

    [Post("/api/election-rounds/{electionRoundId}/quick-reports")]
    Task SubmitQuickReport(
        [AliasAs("electionRoundId")] string electionRoundId,
        [Body] QuickReportRequest quickReport,
        [Authorize] string token);

    [Multipart]
    [Post("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/attachments")]
    Task SubmitQuickReportAttachment(
        [AliasAs("electionRoundId")] string electionRoundId,
        [AliasAs("QuickReportId")] string quickReportId,
        [AliasAs("Id")] string id,
        [AliasAs("Attachment")] StreamPart attachment,
        [Authorize] string token);
}