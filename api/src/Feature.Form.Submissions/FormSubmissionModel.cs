using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Module.Answers.Mappers;
using Module.Answers.Models;

namespace Feature.Form.Submissions;

public record FormSubmissionModel
{
    public required Guid Id { get; init; }
    public required Guid FormId { get; init; }
    public required Guid PollingStationId { get; init; }
    public SubmissionFollowUpStatus FollowUpStatus { get; init; }
    public IReadOnlyList<BaseAnswerModel> Answers { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public int NumberOfNotes { get; init; }
    public int NumberOfAttachments { get; init; }

    public static FormSubmissionModel FromEntity(FormSubmission entity) => new()
    {
        Id = entity.Id,
        PollingStationId = entity.PollingStationId,
        FormId = entity.FormId,
        Answers = entity.Answers
            .Select(AnswerMapper.ToModel)
            .ToList(),
        FollowUpStatus = entity.FollowUpStatus,
        IsCompleted = entity.IsCompleted,
        CreatedAt = entity.CreatedAt,
        LastUpdatedAt = entity.LastUpdatedAt
    };
}
