using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Domain.ValueComparers;

public class ObservationBreaksValueComparer() : ValueComparer<IReadOnlyList<ObservationBreak>>(
    (c1, c2) => c1.SequenceEqual(c2),
    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
    c => c);