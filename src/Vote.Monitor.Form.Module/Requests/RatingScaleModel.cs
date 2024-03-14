using Ardalis.SmartEnum;
namespace Vote.Monitor.Form.Module.Requests;

public class RatingScaleModel : SmartEnum<RatingScaleModel, string>
{
    public static readonly RatingScaleModel OneTo3 = new(nameof(OneTo3), nameof(OneTo3));
    public static readonly RatingScaleModel OneTo4 = new(nameof(OneTo4), nameof(OneTo4));
    public static readonly RatingScaleModel OneTo5 = new(nameof(OneTo5), nameof(OneTo5));
    public static readonly RatingScaleModel OneTo6 = new(nameof(OneTo6), nameof(OneTo6));
    public static readonly RatingScaleModel OneTo7 = new(nameof(OneTo7), nameof(OneTo7));
    public static readonly RatingScaleModel OneTo8 = new(nameof(OneTo8), nameof(OneTo8));
    public static readonly RatingScaleModel OneTo9 = new(nameof(OneTo9), nameof(OneTo9));
    public static readonly RatingScaleModel OneTo10 = new(nameof(OneTo10), nameof(OneTo10));

    private RatingScaleModel(string name, string value) : base(name, value)
    {
    }
}
