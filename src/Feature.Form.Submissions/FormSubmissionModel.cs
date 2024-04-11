using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Answer.Module.Models;

namespace Feature.Form.Submissions;

public record FormSubmissionModel
{
    public required Guid Id { get; init; }
    public required Guid PollingStationId { get; init; }
    public IReadOnlyList<BaseAnswerModel> Answers { get; set; }

    public static FormSubmissionModel FromEntity(FormSubmission entity) => new()
    {
        Id = entity.Id,
        PollingStationId = entity.PollingStationId,
        Answers = entity.Answers
            .Select(AnswerMapper.ToModel)
            .ToList(),
    };
}
