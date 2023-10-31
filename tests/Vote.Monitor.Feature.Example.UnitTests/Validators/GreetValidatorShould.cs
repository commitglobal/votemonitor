namespace Vote.Monitor.Feature.Example.UnitTests.Validators;

public class GreetValidatorShould
{
    private readonly Greet.Validator _validator;

    public GreetValidatorShould()
    {
        _validator = new Greet.Validator();
    }

    [Theory]
    [MemberData(nameof(EmptyStringsTestCases))]

    public void Invalidate_request_with_empty_name(string name)
    {
        var request = new Greet.Request { Name = name };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [MemberData(nameof(InvalidStringLengthTestCases))]

    public void Invalidate_request_with_invalid_name_length(string name)
    {
        var request = new Greet.Request { Name = name };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    public static IEnumerable<object[]> EmptyStringsTestCases =>
        new List<object[]>
        {
            new object[] { null},
            new object[] { ""},
            new object[] { "\t"},
        };
    public static IEnumerable<object[]> InvalidStringLengthTestCases =>
        new List<object[]>
        {
            new object[] { "s".Repeat(1025)},
            new object[] { "s".Repeat(1)},
            new object[] { "s".Repeat(2)},
            new object[] { "s".Repeat(3)},
            new object[] { "s".Repeat(4)}
        };
}
