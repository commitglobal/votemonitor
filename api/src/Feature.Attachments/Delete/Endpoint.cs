using Authorization.Policies.Requirements;
using Feature.Attachments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.Attachments.Delete;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<AttachmentAggregate> repository,
    IRecordAutomation recordAutomation)
    : Endpoint<Request, Results<NoContent, NotFound, BadRequest<ProblemDetails>>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        Summary(s =>
        {
            s.Summary = "Deletes an attachment";
        });
    }

    public override async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.ObserverId, req.Id);
        var attachment = await repository.FirstOrDefaultAsync(specification, ct);

        if (attachment is null)
        {
            return TypedResults.NotFound();
        }

        // Only an uploaded attachment was part of the submission.
        var wasVisible = attachment.IsCompleted && !attachment.IsDeleted;
        attachment.Delete();

        await repository.UpdateAsync(attachment, ct);
        if (wasVisible)
        {
            await recordAutomation.FormSubmissionPartChangedByAuthorAsync(req.ElectionRoundId, attachment.SubmissionId,
                attachment.PollingStationId, attachment.FormId, attachment.MonitoringObserverId, ct);
        }

        return TypedResults.NoContent();
    }
}
