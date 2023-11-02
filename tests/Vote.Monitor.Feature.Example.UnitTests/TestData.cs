namespace Vote.Monitor.Feature.PollingStation.UnitTests;

public class TestData
{
    public static IEnumerable<object[]> EmptyStringsTestCases =>
        new List<object[]>
        {
            new object[] { null},
            new object[] { ""},
            new object[] { " "},
            new object[] { "     "},
            new object[] { "\t"},
        };
}
