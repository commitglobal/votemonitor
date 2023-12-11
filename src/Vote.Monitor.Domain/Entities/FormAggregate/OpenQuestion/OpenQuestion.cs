namespace Vote.Monitor.Domain.Entities.FormAggregate.OpenQuestion;

public class OpenQuestion : BaseQuestion
{
#pragma warning disable CS8618 // Required by Entity Framework
    private OpenQuestion(): base()
    {

    }

    public string Placeholder { get; private set; }
    public bool LongAnswer { get; private set; }
    public OpenQuestionType OpenQuestionType { get; private set; }

    public OpenQuestion(string headline, string subheader, string placeholder, bool longAnswer, OpenQuestionType openQuestionType) : base(headline, subheader, QuetionType.OpenText)
    {
        Placeholder = placeholder;
        LongAnswer = longAnswer;
        OpenQuestionType = openQuestionType;
    }
}
