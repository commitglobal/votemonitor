using Refit;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Clients.MonitoringObserver;

public interface IMonitoringObserverApi
{
    [Post("/api/election-rounds/{electionRoundId}/form-submissions")]
    Task SubmitForm([AliasAs("electionRoundId")] Guid electionRoundId, [Body] SubmissionRequest submission, [Authorize] string token);
}