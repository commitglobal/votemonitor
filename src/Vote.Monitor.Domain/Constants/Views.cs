namespace Vote.Monitor.Domain.Constants;

public static class Views
{
    /// <summary>
    /// A view to retrieve list of polling stations visited.
    /// A polling station visit is any interaction of an observer with data related to polling station
    /// <para>Example of interactions:</para>
    /// <list type="bullet">
    ///     <item>Filling in polling station information</item>
    ///     <item>Filling in any form</item>
    ///     <item>Adding a note</item>
    ///     <item>Adding an attachment</item>
    /// </list>
    /// </summary>
    public const string PollingStationVisits = "PollingStationVisits";
}
