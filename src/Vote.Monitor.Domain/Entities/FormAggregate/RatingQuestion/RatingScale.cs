using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormAggregate.RatingQuestion;

public class RatingScale : SmartEnum<RatingScale, string>
{
    public static readonly RatingScale Range3 = new(nameof(Range3), nameof(Range3));
    public static readonly RatingScale Range5 = new(nameof(Range5), nameof(Range5));
    public static readonly RatingScale Range10 = new(nameof(Range10), nameof(Range10));

    public RatingScale(string name, string value) : base(name, value)
    {
    }
}

