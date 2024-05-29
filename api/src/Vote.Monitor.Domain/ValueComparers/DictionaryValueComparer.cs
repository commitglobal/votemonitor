using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Vote.Monitor.Domain.ValueComparers;

public class DictionaryValueComparer : ValueComparer<Dictionary<string, string>>
{
    public DictionaryValueComparer() : base((c1, c2) => c1.Equals(c2),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c)
    {
    }
}
