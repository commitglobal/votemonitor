namespace Vote.Monitor.Domain.Entities.FormAggregate.RatingQuestion;
public class RatingQuestion : BaseQuestion
{
#pragma warning disable CS8618 // Required by Entity Framework
    private RatingQuestion() : base()
    {

    }

    public Guid Id { get; private set; }

    public RatingScale Scale { get; private set; }

    public RatingQuestion(Guid id, string headline, string subheader, RatingScale scale) : base(headline, subheader, QuetionType.Rating)
    {
        Id = id;
        Scale = scale;
    }
}
