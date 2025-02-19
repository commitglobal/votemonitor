using Refit;
using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;

namespace SubmissionsFaker.Clients.NgoAdmin;

public interface INgoAdminApi
{
    [Post("/api/election-rounds/{electionRoundId}/forms")]
    Task<ResponseWithId> CreateForm([AliasAs("electionRoundId")] Guid electionRoundId, [Body] CreateFormRequest form);

    [Post("/api/election-rounds/{electionRoundId}/forms/{id}:publish")]
    Task PublishForm([AliasAs("electionRoundId")] Guid electionRoundId,
        [AliasAs("id")] Guid id);
}