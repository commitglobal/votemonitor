using Ardalis.SmartEnum;

namespace Vote.Monitor.Core.Models;

[JsonConverter(typeof(SmartEnumValueConverter<QuestionsAnsweredFilter, string>))]
public sealed class QuestionsAnsweredFilter : SmartEnum<QuestionsAnsweredFilter, string>
{
    public static readonly QuestionsAnsweredFilter None = new(nameof(None), nameof(None));
    public static readonly QuestionsAnsweredFilter Some = new(nameof(Some), nameof(Some));
    public static readonly QuestionsAnsweredFilter All = new(nameof(All), nameof(All));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="QuestionsAnsweredFilter" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out QuestionsAnsweredFilter result)
    {
        return TryFromValue(value, out result);
    }

    private QuestionsAnsweredFilter(string name, string value) : base(name, value)
    {
    }
}