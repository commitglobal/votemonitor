using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.ValueComparers;

public class TranslatedStringValueComparer : ValueComparer<TranslatedString>
{
    public TranslatedStringValueComparer() : base((c1, c2) => c1.Equals(c2),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c)
    {
    }
}
public class LanguagesTranslationStatusValueComparer : ValueComparer<LanguagesTranslationStatus>
{
    public LanguagesTranslationStatusValueComparer() : base((c1, c2) => c1.Equals(c2),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c)
    {
    }
}
