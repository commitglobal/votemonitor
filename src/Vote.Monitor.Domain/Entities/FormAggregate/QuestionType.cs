using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormAggregate;
public sealed class QuetionType : SmartEnum<QuetionType, string>
{
    /// <summary>
    /// Open question
    /// </summary>
    public static readonly QuetionType OpenText = new(nameof(OpenText), nameof(OpenText));

    /// <summary>
    /// Single response from a list of options
    /// </summary>
    public static readonly QuetionType SingleResponse = new(nameof(SingleResponse), nameof(SingleResponse));

    /// <summary>
    /// Multiple responses from a list of options
    /// </summary>
    public static readonly QuetionType MultiResponse = new(nameof(MultiResponse), nameof(MultiResponse));

    /// <summary>
    /// Rating question
    /// </summary>
    public static readonly QuetionType Rating = new(nameof(Rating), nameof(Rating));

    private QuetionType(string name, string value) : base(name, value)
    {
    }
}
