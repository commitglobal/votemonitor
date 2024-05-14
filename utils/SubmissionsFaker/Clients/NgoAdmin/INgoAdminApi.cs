using Refit;
using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;

namespace SubmissionsFaker.Clients.NgoAdmin;

public interface INgoAdminApi
{
    [Post("/api/election-rounds/{electionRoundId}/forms")]
    Task<CreateResponse> CreateForm([AliasAs("electionRoundId")] Guid electionRoundId, [Body] NewForm form, [Authorize] string token);

    [Put("/api/election-rounds/{electionRoundId}/forms/{id}")]
    Task UpdateForm([AliasAs("electionRoundId")] Guid electionRoundId,
        [AliasAs("id")] Guid id,
        [Body] UpdateForm form,
        [Authorize] string token);
}