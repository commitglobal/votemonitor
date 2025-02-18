﻿using Refit;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Fakers;

namespace SubmissionsFaker.Clients.MonitoringObserver;

public interface IMonitoringObserverApi
{
    [Post("/api/election-rounds/{electionRoundId}/form-submissions")]
    Task SubmitForm(
        [AliasAs("electionRoundId")] Guid electionRoundId,
        [Body] SubmissionRequest submission,
        [Authorize] string token);

    [Multipart]
    [Post("/api/election-rounds/{electionRoundId}/attachments")]
    Task<IApiResponse> SubmitAttachment(
        [AliasAs("electionRoundId")] Guid electionRoundId,
        [AliasAs("PollingStationId")] string pollingStationId,
        [AliasAs("Id")] string id,
        [AliasAs("FormId")] string formId,
        [AliasAs("QuestionId")] string questionId,
        [AliasAs("Attachment")] StreamPart attachment,
        [Authorize] string token);

    [Post("/api/election-rounds/{electionRoundId}/notes")]
    Task SubmitNote(
        [AliasAs("electionRoundId")] Guid electionRoundId,
        [Body] NoteRequest note,
        [Authorize] string token);

    [Post("/api/election-rounds/{electionRoundId}/quick-reports")]
    Task SubmitQuickReport(
        [AliasAs("electionRoundId")] Guid electionRoundId,
        [Body] QuickReportRequest quickReport,
        [Authorize] string token);

    [Multipart]
    [Post("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/attachments")]
    Task SubmitQuickReportAttachment(
        [AliasAs("electionRoundId")] Guid electionRoundId,
        [AliasAs("QuickReportId")] string quickReportId,
        [AliasAs("Id")] string id,
        [AliasAs("Attachment")] StreamPart attachment,
        [Authorize] string token);

    [Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information")]
    Task SubmitPSIForm(
        [AliasAs("electionRoundId")] Guid electionRoundId,
        [AliasAs("pollingStationId")] Guid pollingStationId,
        [Body] PSISubmissionRequest submission,
        [Authorize] string token);
    
    [Post("/api/election-rounds/{electionRoundId}/incident-reports")]
    Task SubmitIncidentReport(
        [AliasAs("electionRoundId")] Guid electionRoundId,
        [Body] IncidentReportRequest incidentReport,
        [Authorize] string token);
}