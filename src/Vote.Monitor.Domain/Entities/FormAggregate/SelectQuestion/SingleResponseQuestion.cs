namespace Vote.Monitor.Domain.Entities.FormAggregate.SelectQuestion;

public class SingleResponseQuestion : BaseQuestion
{
#pragma warning disable CS8618 // Required by Entity Framework
    private SingleResponseQuestion() : base()
    {

    }

    public Guid Id { get; private set; }
    public List<QuestionOption> Options { get; private set; }

    public SingleResponseQuestion(Guid id, string headline, string subheader, List<QuestionOption> options) : base(headline, subheader, QuetionType.SingleResponse)
    {
        Id = id;
        Options = options;
    }
}

