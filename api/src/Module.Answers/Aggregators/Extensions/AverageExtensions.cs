namespace Module.Answers.Aggregators.Extensions;

internal static class AverageExtensions
{
    /// <summary>
    /// Computes average using incremental averaging.
    /// </summary>
    /// <remarks>
    /// Based on <see href="https://math.stackexchange.com/questions/106700/incremental-averaging" >this article</see>
    /// </remarks>
    /// <param name="average">Previous value of average</param>
    /// <param name="value">New value to use in computation</param>
    /// <param name="answersCount">Total answers count including newly received value</param>
    /// <returns>Average value</returns>
    internal static decimal RecomputeAverage(this decimal average, int value, int answersCount)
    {
        var newAverage = average + (value - average) / answersCount;

        return newAverage;
    }
}
