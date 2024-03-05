using System.Text.Json.Serialization;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public class FormSection
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Title { get; private set; }
    public IReadOnlyList<BaseQuestion> Questions { get; private set; }


    public FormSection()
    {

    }

    [JsonConstructor]
    public FormSection(Guid id, string code, TranslatedString title, IReadOnlyList<BaseQuestion> questions)
    {
        Id = id;
        Code = code;
        Title = title;
        Questions = questions;
    }

    public static FormSection Create(string code, TranslatedString title)
    {
        return new FormSection(Guid.NewGuid(), code, title, []);
    }

    public TextInputQuestion AddTextInputQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString inputPlaceholder)
    {
        var textInputQuestion = TextInputQuestion.Create(id, code, text, helptext, inputPlaceholder);
        Questions = Questions.Concat([textInputQuestion]).ToList();
        return textInputQuestion;
    }

    public NumberInputQuestion AddNumberInputQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? inputPlaceholder)
    {
        var numberInputQuestion = NumberInputQuestion.Create(id, code, text, helptext, inputPlaceholder);
        Questions = Questions.Concat([numberInputQuestion]).ToList().AsReadOnly();
        return numberInputQuestion;
    }

    public DateInputQuestion AddDateInputQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext)
    {
        var dateInputQuestion = DateInputQuestion.Create(id, code, text, helptext);
        Questions = Questions.Concat([dateInputQuestion]).ToList().AsReadOnly();
        return dateInputQuestion;
    }

    public SingleSelectQuestion AddSingleSelectQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext)
    {
        var singleSelectQuestion = SingleSelectQuestion.Create(id, code, text, helptext);
        Questions = Questions.Concat([singleSelectQuestion]).ToList().AsReadOnly();
        return singleSelectQuestion;
    }

    public MultiSelectQuestion AddMultiSelectQuestion(
        Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext)
    {
        var multiSelectQuestion = MultiSelectQuestion.Create(id, code, text, helptext);
        Questions = Questions.Concat([multiSelectQuestion]).ToList().AsReadOnly();
        return multiSelectQuestion;
    }

    public RatingQuestion AddRatingQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        RatingScale scale)
    {
        var ratingQuestion = RatingQuestion.Create(id, code, text, helptext, scale);
        Questions = Questions.Concat([ratingQuestion]).ToList().AsReadOnly();
        return ratingQuestion;
    }

    public GridQuestion AddGridQuestion(
        Guid id,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? scalePlaceholder,
        RatingScale scale,
        bool hasNotKnownColumn)
    {
        var gridQuestion = GridQuestion.Create(id, text, helptext, scalePlaceholder, scale, hasNotKnownColumn);
        Questions = Questions.Concat([gridQuestion]).ToList().AsReadOnly();
        return gridQuestion;
    }
}
