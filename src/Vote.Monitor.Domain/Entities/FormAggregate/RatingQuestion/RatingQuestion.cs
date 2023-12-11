namespace Vote.Monitor.Domain.Entities.FormAggregate.RatingQuestion;
public class RatingQuestion : BaseQuestion
{
#pragma warning disable CS8618 // Required by Entity Framework
    private RatingQuestion(): base()
    {

    }

    public RatingScale Scale { get; private set; }

    public RatingQuestion(string headline, string subheader, RatingScale scale) : base(headline, subheader, QuetionType.Rating)
    {
        Scale = scale;
    }
}
