using Microsoft.EntityFrameworkCore;
using Module.Answers.Mappers;
using Vote.Monitor.Domain;

namespace Feature.Form.Submissions.ListMy;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:my");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets all form submissions by an observer";
            s.Description = "Allows filtering by polling station";
        });

        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }


        var submissions = await context.FormSubmissions.Select(fs => new
            {
                fs.Id,
                fs.PollingStationId,
                fs.FormId,
                fs.Answers,
                fs.FollowUpStatus,
                fs.IsCompleted,
                fs.CreatedAt,
                fs.LastUpdatedAt,
                NumberOfAttachments =
                    context.Attachments.Count(a =>
                        a.SubmissionId == fs.Id && a.MonitoringObserverId == fs.MonitoringObserverId &&
                        a.IsCompleted && !a.IsDeleted),
                NumberOfNotes = context.Notes.Count(n =>
                    n.SubmissionId == fs.Id && n.MonitoringObserverId == fs.MonitoringObserverId),
            }).AsNoTracking()
            .ToListAsync(ct);
        

        return TypedResults.Ok(new Response
        {
            Submissions = submissions.Select(entity =>     new FormSubmissionModel(){
                Id = entity.Id,
                PollingStationId = entity.PollingStationId,
                FormId = entity.FormId,
                Answers = entity.Answers
                    .Select(AnswerMapper.ToModel)
                    .ToList(),
                FollowUpStatus = entity.FollowUpStatus,
                IsCompleted = entity.IsCompleted,
                CreatedAt = entity.CreatedAt,
                LastUpdatedAt = entity.LastUpdatedAt,
                NumberOfAttachments= entity.NumberOfAttachments,
                NumberOfNotes= entity.NumberOfNotes,
                
                
            }).ToList()
        });
    }
}
