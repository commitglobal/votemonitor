using Ardalis.SmartEnum;

namespace Vote.Monitor.Core.Models;

public sealed class SortOrder : SmartEnum<SortOrder, string>
{
    public static readonly SortOrder Asc = new(nameof(Asc), nameof(Asc));
    public static readonly SortOrder Desc = new(nameof(Desc), nameof(Desc));

    private SortOrder(string name, string value) : base(name, value)
    {
    }
}
