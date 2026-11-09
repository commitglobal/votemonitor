using Feature.Review.Services;
using Module.Answers.Mappers;
using Module.Answers.Models;
using Vote.Monitor.Domain.Entities.AnswerHistoryAggregate;

namespace Feature.Review.ListAnswerHistory;

/// <summary>One change to one answer: the question, its answer before and after (null when added or cleared), when.</summary>
public record AnswerChangeModel
{
    public required Guid Id { get; init; }
    public required Guid QuestionId { get; init; }
    public BaseAnswerModel? PreviousAnswer { get; init; }
    public BaseAnswerModel? CurrentAnswer { get; init; }
    public required DateTime ChangedAt { get; init; }

    public static AnswerChangeModel FromEntity(AnswerChange change) => new()
    {
        Id = change.Id,
        QuestionId = change.QuestionId,
        PreviousAnswer = change.PreviousAnswer is null ? null : AnswerMapper.ToModel(change.PreviousAnswer),
        CurrentAnswer = change.CurrentAnswer is null ? null : AnswerMapper.ToModel(change.CurrentAnswer),
        ChangedAt = change.ChangedAt
    };
}

/// <summary>The record's answer history and, for the caller's NGO, when its author last changed it after a review.</summary>
public record AnswerHistoryModel
{
    public required AnswerChangeModel[] Changes { get; init; }
    public DateTime? ChangedAfterReviewAt { get; init; }
}

public class Endpoint(
    IAuthorizationService authorizationService,
    IReviewService reviewService,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<AnswerHistoryModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/reviews/{recordType}/{recordId}/answer-history");
        DontAutoTag();
        Options(x => x.WithTags("reviews"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Lists how the record's observer changed its answers, newest first";
            s.Description = "Form submissions and incident reports; other record types have no answer history.";
        });
    }

    public override async Task<Results<Ok<AnswerHistoryModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        if (!await authorizationService.CanWorkOnReportsAsync(User, req.ElectionRoundId, req.RecordType))
        {
            return TypedResults.NotFound();
        }

        var report = await reviewService.ResolveAsync(req.ElectionRoundId, req.NgoId, req.RecordType, req.RecordId, ct);
        if (report is null)
        {
            return TypedResults.NotFound();
        }

        var changes = await context.AnswerChanges
            .AsNoTracking()
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.RecordType == req.RecordType
                        && x.RecordId == req.RecordId)
            .OrderByDescending(x => x.ChangedAt)
            .ToListAsync(ct);

        return TypedResults.Ok(new AnswerHistoryModel
        {
            Changes = changes.Select(AnswerChangeModel.FromEntity).ToArray(),
            ChangedAfterReviewAt = report.Workflow?.ChangedAfterReviewAt
        });
    }
}
