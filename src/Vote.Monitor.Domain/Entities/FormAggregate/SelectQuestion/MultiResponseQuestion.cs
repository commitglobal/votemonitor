namespace Vote.Monitor.Domain.Entities.FormAggregate.SelectQuestion;

public class MultiResponseQuestion : BaseQuestion
{
#pragma warning disable CS8618 // Required by Entity Framework
    private MultiResponseQuestion()
    {

    }

    public List<Option> Options { get; private set; }

    public MultiResponseQuestion(string headline, string subheader, List<Option> options) : base(headline, subheader, QuetionType.MultiResponse)
    {
        Options = options;
    }
}
