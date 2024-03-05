using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Models;

public class GridQuestionRowModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }
    public TranslatedString Text { get; init; }
    public TranslatedString? Helptext { get; init; }

    public static GridQuestionRowModel FromEntity(GridQuestionRow row) =>
        new()
        {
            Id = row.Id,
            Code = row.Code,
            Text = row.Text,
            Helptext = row.Helptext
        };
}
