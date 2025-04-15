using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Models;

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
