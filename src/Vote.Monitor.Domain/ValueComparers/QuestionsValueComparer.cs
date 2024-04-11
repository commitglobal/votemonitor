using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.ValueComparers;

public class QuestionsValueComparer : ValueComparer<IReadOnlyList<BaseQuestion>>
{
    public QuestionsValueComparer() : base((c1, c2) => c1.SequenceEqual(c2),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c)
    {
    }
}
