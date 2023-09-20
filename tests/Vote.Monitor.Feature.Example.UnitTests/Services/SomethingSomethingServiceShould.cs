using Vote.Monitor.Feature.Example.Services;

namespace Vote.Monitor.Feature.Example.UnitTests.Services;

public class SomethingSomethingServiceShould
{
    private readonly SomethingSomethingService _service;

    public SomethingSomethingServiceShould()
    {
        _service = new SomethingSomethingService();
    }

    [Theory]
    [MemberData(nameof(InvalidStringsTestCases))]
    public async Task Return_error_result_when_empty_parameter(string parameter)
    {
        var result = await _service.DoSomethingAsync(parameter);

        result.Should().BeOfType<SomethingResult.Error>();
    }


    public static IEnumerable<object[]> InvalidStringsTestCases =>
        new List<object[]>
        {
            new object[] { null},
            new object[] { ""},
            new object[] { "\t"},
        };
}
