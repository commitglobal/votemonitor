namespace Vote.Monitor.Domain.Entities.FormAggregate.SelectQuestion;

public class SingleResponseQuestion : BaseQuestion
{
#pragma warning disable CS8618 // Required by Entity Framework
    private SingleResponseQuestion() : base()
    {

    }

    public List<Option> Options { get; private set; }

    public SingleResponseQuestion(string headline, string subheader, List<Option> options) : base(headline, subheader, QuetionType.SingleResponse)
    {
        Options = options;
    }
}

