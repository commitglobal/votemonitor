using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormAggregate.OpenQuestion;

public sealed class OpenQuestionType : SmartEnum<OpenQuestionType, string>
{
    /// <summary>
    /// Open question where users can respond with text
    /// </summary>
    public static readonly OpenQuestionType Text = new(nameof(Text), nameof(Text));

    /// <summary>
    /// Open question where users can only respond with a number
    /// </summary>
    public static readonly OpenQuestionType Numeric = new(nameof(Numeric), nameof(Numeric));

  
    private OpenQuestionType(string name, string value) : base(name, value)
    {
    }
}
