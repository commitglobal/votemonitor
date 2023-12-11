namespace Vote.Monitor.Domain.Entities.FormAggregate;

public class BaseQuestion
{
#pragma warning disable CS8618 // Required by Entity Framework
    protected BaseQuestion()
    {

    }

    public string Headline { get; private set; }
    public string Subheader { get; private set; }
    public QuetionType Type { get; private set; }

    protected BaseQuestion(string headline, string subheader, QuetionType type)
    {
        Headline = headline;
        Subheader = subheader;
        Type = type;
    }
}
