using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate.OpenQuestion;

namespace Vote.Monitor.Api.Feature.Forms.Models;

public class OpenTextQuestionModel : BaseQuestionModel
{
    public string Placeholder { get; }
    public bool LongAnswer { get; }

    public OpenQuestionType OpenQuestionType { get; set; }


    public OpenTextQuestionModel(string headline, string subheader, bool isOptional, bool isFlagged, string placeholder, bool longAnswer, OpenQuestionType openQuestionType) : base(headline, subheader, QuetionType.OpenText, isOptional, isFlagged)
    {
        Placeholder = placeholder;
        LongAnswer = longAnswer;
        OpenQuestionType = openQuestionType;
    }
}
