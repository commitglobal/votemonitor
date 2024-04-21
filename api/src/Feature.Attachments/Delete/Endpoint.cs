using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Feature.Attachments.Delete;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<AttachmentAggregate> _repository;

    public Endpoint(IAuthorizationService authorizationService,
        IRepository<AttachmentAggregate> repository)
    {
        _repository = repository;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        Summary(s => {
            s.Summary = "Deletes an attachment";
        });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var attachment = await _repository.GetByIdAsync(req.Id, ct);
        if (attachment is null)
        {
            return TypedResults.NotFound();
        }

        attachment.Delete();

        await _repository.UpdateAsync(attachment, ct);
        
        return TypedResults.NoContent();
    }
}
