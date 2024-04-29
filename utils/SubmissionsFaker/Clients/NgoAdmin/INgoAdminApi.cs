using Refit;
using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;

namespace SubmissionsFaker.Clients.NgoAdmin;

public interface INgoAdminApi
{
    [Post("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgoId}/forms")]
    Task<CreateResponse> CreateForm([AliasAs("electionRoundId")] Guid electionRoundId, [AliasAs("monitoringNgoId")] Guid monitoringNgoId, [Body] NewForm form, [Authorize] string token);

    [Put("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgoId}/forms/{id}")]
    Task UpdateForm([AliasAs("electionRoundId")] Guid electionRoundId,
        [AliasAs("monitoringNgoId")] Guid monitoringNgoId,
        [AliasAs("id")] Guid id,
        [Body] UpdateForm form, [Authorize] string token);
}