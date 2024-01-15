namespace Vote.Monitor.TestUtils;

public class TestData
{
    public static IEnumerable<object[]> EmptyAndNullStringsTestCases =>
        new List<object[]>
        {
            new object[] { null},
            new object[] { ""},
            new object[] { " "},
            new object[] { "     "},
            new object[] { "\t"},
        };

    public static IEnumerable<object[]> EmptyStringsTestCases =>
        new List<object[]>
        {
            new object[] { ""},
            new object[] { " "},
            new object[] { "     "},
            new object[] { "\t"},
        };
}
