using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Form.Module.Models;

namespace Feature.Form.Submissions.GetById;

public record Response
{
    public required Guid Id { get; init; }
    public required Guid FormId { get; init; }
    public required Guid PollingStationId { get; init; }
    public IReadOnlyList<BaseQuestionModel> Questions { get; init; }
    public IReadOnlyList<BaseAnswerModel> Answers { get; init; }
    public IReadOnlyList<NoteModel> Notes { get; init; }
    public IReadOnlyList<MediaFileModel> MediaFiles { get; init; }

    public static Response FromEntity(FormSubmission entity) => new()
    {
        Id = entity.Id,
        PollingStationId = entity.PollingStationId,
        FormId = entity.FormId,
        Answers = entity.Answers
            .Select(AnswerMapper.ToModel)
            .ToList(),
    };
}


public record NoteModel
{
    public Guid QuestionId { get; init; }
    public string Text { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record MediaFileModel
{
    public Guid QuestionId { get; init; }
    public string PresignedUrl { get; init; }
    public DateTime CreatedAt { get; init; }
}
