namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.Specifications;

public class GetFormTemplateSpecificationTests
{
    [Fact]
    public void GetFormTemplateSpecification_UsesExactMatchOnCode()
    {
        // Arrange
        var formTemplate = new FormTemplateAggregateFaker().Generate();

        var testCollection = new FormTemplateAggregateFaker()
            .Generate(500)
            .Union(new[] { formTemplate })
            .Union(new FormTemplateAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetFormTemplateSpecification(formTemplate.Code, formTemplate.FormTemplateType);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1); // Expecting only one item in the result
        result.Should().Contain(formTemplate);
    }

    [Fact]
    public void GetFormTemplateSpecification_MatchesByCodeButNotId()
    {
        // Arrange
        var formTemplate1 = new FormTemplateAggregateFaker(code: "A", status: FormTemplateStatus.Published).Generate();
        var formTemplate2 = new FormTemplateAggregateFaker(code: "A", status: FormTemplateStatus.Published).Generate();

        var testCollection = new FormTemplateAggregateFaker()
            .Generate(500)
            .Union(new[] { formTemplate1, formTemplate2 })
            .Union(new FormTemplateAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetFormTemplateSpecification(formTemplate1.Id, formTemplate1.Code, formTemplate1.FormTemplateType);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1); // Expecting only one item in the result
        result.Should().Contain(formTemplate2);
    }
}
