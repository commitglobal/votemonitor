using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.Auditing;

public sealed class TrailType : SmartEnum<TrailType, string>
{
    public static readonly TrailType Create = new(nameof(Create), nameof(Create));
    public static readonly TrailType Update = new(nameof(Update), nameof(Update));
    public static readonly TrailType Delete = new(nameof(Delete), nameof(Delete));

    private TrailType(string name, string value) : base(name, value)
    {
    }
}
