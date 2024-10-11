using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Answer.Module.Models;

namespace Feature.Form.Submissions;

public record FormSubmissionModel
{
    public required Guid Id { get; init; }
    public required Guid FormId { get; init; }
    public required Guid PollingStationId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<SubmissionFollowUpStatus, string>))]
    public SubmissionFollowUpStatus FollowUpStatus { get; init; }

    public IReadOnlyList<BaseAnswerModel> Answers { get; init; }
    public bool IsCompleted { get; init; }

    public static FormSubmissionModel FromEntity(FormSubmission entity) => new()
    {
        Id = entity.Id,
        PollingStationId = entity.PollingStationId,
        FormId = entity.FormId,
        Answers = entity.Answers
            .Select(AnswerMapper.ToModel)
            .ToList(),
        FollowUpStatus = entity.FollowUpStatus,
        IsCompleted = entity.IsCompleted
    };
}