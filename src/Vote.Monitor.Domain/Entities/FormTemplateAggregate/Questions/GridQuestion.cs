using System.Text.Json.Serialization;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

public class GridQuestion : BaseQuestion
{
    public Guid Id { get; private set; }
    public TranslatedString? ScalePlaceholder { get; private set; }
    public bool HasNotKnownColumn { get; private set; }
    public RatingScale Scale { get; private set; }
    public IReadOnlyList<GridQuestionRow> Rows { get; private set; }

    [JsonConstructor]
    internal GridQuestion(Guid id,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? scalePlaceholder,
        RatingScale scale,
        bool hasNotKnownColumn,
        IReadOnlyList<GridQuestionRow> rows) : base(text, helptext)
    {
        Id = id;
        ScalePlaceholder = scalePlaceholder;
        Scale = scale;
        Rows = rows;
        HasNotKnownColumn = hasNotKnownColumn;
    }

    public GridQuestionRow AddRow(Guid id, string code, TranslatedString text, TranslatedString? helptext)
    {
        var row = GridQuestionRow.Create(id, code, text, helptext);
        Rows = Rows.Union([row]).ToList().AsReadOnly();
        return row;
    }

    public static GridQuestion Create(
        Guid id,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? scalePlaceholder,
        RatingScale scale,
        bool hasNotKnownColumn) => new(id, text, helptext, scalePlaceholder, scale, hasNotKnownColumn, []);
}
