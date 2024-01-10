namespace Vote.Monitor.Domain.Entities.FormAggregate.SelectQuestion;

public class MultiResponseQuestion : BaseQuestion
{
#pragma warning disable CS8618 // Required by Entity Framework
    private MultiResponseQuestion()
    {
    }

    public Guid Id { get; private set; }
    public List<QuestionOption> Options { get; private set; }

    public MultiResponseQuestion(Guid id, string headline, string subheader, List<QuestionOption> options) : base(headline, subheader, QuetionType.MultiResponse)
    {
        Options = options;
        Id = id;
    }
}
