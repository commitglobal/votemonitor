using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public class OpenQuestionModel : BaseQuestionModel
{
    public string Placeholder { get; }
    public bool LongAnswer { get; }

    public OpenQuestionType OpenQuestionType { get; set; }

    public OpenQuestionModel(Guid id,
        string headline,
        string subheader,
        string placeholder,
        bool longAnswer,
        OpenQuestionType openQuestionType) : base(id, headline, subheader, QuetionType.OpenText)
    {
        Placeholder = placeholder;
        LongAnswer = longAnswer;
        OpenQuestionType = openQuestionType;
    }
}
