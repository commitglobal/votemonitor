namespace Feature.ElectionRounds;

/// <summary>The earliest start date an election round can be given.</summary>
public static class ElectionRoundStartDate
{
    public const string InThePastMessage = "Election start date cannot be in the past.";

    /// <summary>
    /// Today somewhere on Earth: the date west of UTC (down to UTC-12) can be one day behind the UTC date, so the day
    /// before the UTC date is still accepted.
    /// </summary>
    public static DateOnly EarliestAllowed(ITimeProvider timeProvider) => timeProvider.UtcNowDate.AddDays(-1);
}
