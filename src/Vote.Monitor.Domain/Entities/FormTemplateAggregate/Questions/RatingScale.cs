using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

public class RatingScale : SmartEnum<RatingScale, string>
{
    public static readonly RatingScale OneTo3 = new(nameof(OneTo3), nameof(OneTo3));
    public static readonly RatingScale OneTo4 = new(nameof(OneTo4), nameof(OneTo4));
    public static readonly RatingScale OneTo5 = new(nameof(OneTo5), nameof(OneTo5));
    public static readonly RatingScale OneTo6 = new(nameof(OneTo6), nameof(OneTo6));
    public static readonly RatingScale OneTo7 = new(nameof(OneTo7), nameof(OneTo7));
    public static readonly RatingScale OneTo8 = new(nameof(OneTo8), nameof(OneTo8));
    public static readonly RatingScale OneTo9 = new(nameof(OneTo9), nameof(OneTo9));
    public static readonly RatingScale OneTo10 = new(nameof(OneTo10), nameof(OneTo10));

    public RatingScale(string name, string value) : base(name, value)
    {
    }
}
