using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Models;

public class SelectOptionModel
{
    public Guid Id { get; init; }
    public TranslatedString Text { get; init; }
    public bool IsFlagged { get; init; }
    public bool IsFreeText { get; init; }
    public static SelectOptionModel FromEntity(SelectOption option) =>
        new()
        {
            Id = option.Id,
            Text = option.Text,
            IsFlagged = option.IsFlagged,
            IsFreeText = option.IsFreeText
        };
}
