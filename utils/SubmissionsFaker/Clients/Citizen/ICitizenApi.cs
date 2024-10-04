using Refit;
using SubmissionsFaker.Clients.Citizen.Models;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Clients.Citizen;

public interface ICitizenApi
{
    [Post("/api/election-rounds/{electionRoundId}/citizen-reports")]
    Task SubmitForm(
        [AliasAs("electionRoundId")] string electionRoundId,
        [Body] CitizenReportRequest citizenReport);

    // [Multipart]
    // [Post("/api/election-rounds/{electionRoundId}/attachments")]
    // Task<IApiResponse> SubmitAttachment(
    //     [AliasAs("ElectionRoundId")] string electionRoundId,
    //     [AliasAs("CitizenReportId")] string pollingStationId,
    //     [AliasAs("Id")] string id,
    //     [AliasAs("FormId")] string formId,
    //     [AliasAs("QuestionId")] string questionId,
    //     [AliasAs("Attachment")] StreamPart attachment);

    [Post("/api/election-rounds/{electionRoundId}/citizen-report-notes")]
    Task SubmitNote(
        [AliasAs("electionRoundId")] string electionRoundId,
        [Body] CitizenReportNoteRequest note);
}