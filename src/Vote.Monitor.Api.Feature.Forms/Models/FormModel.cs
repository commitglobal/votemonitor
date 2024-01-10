using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public record FormModel
{
    public Guid Id { get; init; }
    public required string Code { get; init; }
    public required Guid LanguageId { get; init; }
    public required string Description { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<FormStatus, string>))]
    public required FormStatus Status { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }

    public required List<BaseQuestionModel> Questions { get; init; } = new();
}
