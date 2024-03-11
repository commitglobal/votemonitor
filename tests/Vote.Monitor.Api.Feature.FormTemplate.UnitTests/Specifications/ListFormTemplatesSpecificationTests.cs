namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.Specifications;

public class ListFormTemplatesSpecificationTests
{
    private const string DefaultCode = "ABC";
    private readonly FormTemplateStatus DefaultStatus = FormTemplateStatus.Drafted;

    [Fact]
    public void ListFormTemplatesSpecification_PaginatesCorrectly()
    {
        // Arrange
        var formTemplate1 = new FormTemplateAggregateFaker(index: 101, status: DefaultStatus).Generate();
        var formTemplate2 = new FormTemplateAggregateFaker(index: 102, status: DefaultStatus).Generate();

        var testCollection = Enumerable.Range(1, 100)
            .Select(idx => new FormTemplateAggregateFaker(index: idx, status: DefaultStatus).Generate())
        .Union(new[] { formTemplate1, formTemplate2 })
        .ToList();

        var request = new List.Request
        {
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListFormTemplatesSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(formTemplate1, cfg => cfg.ExcludingMissingMembers());
        result.Should().ContainEquivalentOf(formTemplate2, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public void ListFormTemplatesSpecification_AppliesCorrectFilters_WhenNameFilterApplied()
    {
        // Arrange
        var formTemplate1 = new FormTemplateAggregateFaker(index: 101, code: DefaultCode, status: DefaultStatus).Generate();
        var formTemplate2 = new FormTemplateAggregateFaker(index: 102, code: DefaultCode, status: DefaultStatus).Generate();

        var testCollection = Enumerable
        .Range(1, 100)
            .Select(index => new FormTemplateAggregateFaker(index: index, status: DefaultStatus).Generate())
            .Union(new[] { formTemplate1, formTemplate2 })
            .ToList();

        var request = new List.Request
        {
            CodeFilter = DefaultCode
        };

        var spec = new ListFormTemplatesSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(formTemplate1, cfg => cfg.ExcludingMissingMembers());
        result.Should().ContainEquivalentOf(formTemplate2, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public void ListFormTemplatesSpecification_AppliesCorrectFilters_WhenStatusFilterApplied()
    {
        // Arrange
        var formTemplate1 = new FormTemplateAggregateFaker(index: 101, code: DefaultCode, status: FormTemplateStatus.Published).Generate();
        var formTemplate2 = new FormTemplateAggregateFaker(index: 102, code: DefaultCode, status: FormTemplateStatus.Published).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(index => new FormTemplateAggregateFaker(index: index, status: FormTemplateStatus.Drafted).Generate())
            .Union(new[] { formTemplate1, formTemplate2 })
            .ToList();

        var request = new List.Request
        {
            Status = FormTemplateStatus.Published
        };

        var spec = new ListFormTemplatesSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(formTemplate1, cfg => cfg.ExcludingMissingMembers());
        result.Should().ContainEquivalentOf(formTemplate2, cfg => cfg.ExcludingMissingMembers());
    }

    [Theory]
    [InlineData("C1")]
    [InlineData("C2")]
    public void ListFormTemplatesSpecification_AppliesCorrectFilters_WhenPartialFilterApplied(string searchString)
    {
        // Arrange
        var formTemplate1 = new FormTemplateAggregateFaker(index: 101, code: searchString, status: DefaultStatus).Generate();
        var formTemplate2 = new FormTemplateAggregateFaker(index: 102, code: searchString, status: DefaultStatus).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new FormTemplateAggregateFaker(index: idx, code: searchString, status: DefaultStatus).Generate())
            .Union(new[] { formTemplate1, formTemplate2 })
            .ToList();

        var request = new List.Request
        {
            CodeFilter = searchString,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListFormTemplatesSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(formTemplate1, cfg => cfg.ExcludingMissingMembers());
        result.Should().ContainEquivalentOf(formTemplate2, cfg => cfg.ExcludingMissingMembers());
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void ListFormTemplatesSpecification_AppliesDefaultSorting_WhenNoSortColumnSet(string columnName)
    {
        // Arrange
        var formTemplate1 = new FormTemplateAggregateFaker(index: 101).Generate();
        var formTemplate2 = new FormTemplateAggregateFaker(index: 102).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new FormTemplateAggregateFaker(index: idx, status: DefaultStatus).Generate())
            .Union(new[] { formTemplate1, formTemplate2 })
            .ToList();

        var request = new List.Request
        {
            SortColumnName = columnName,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListFormTemplatesSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(formTemplate1, cfg => cfg.ExcludingMissingMembers());
        result.Should().ContainEquivalentOf(formTemplate2, cfg => cfg.ExcludingMissingMembers());
    }

    [Theory]
    [MemberData(nameof(NameSortingTestCases))]
    public void ListFormTemplatesSpecification_AppliesSortingCorrectly(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var formTemplate1 = new FormTemplateAggregateFaker(index: 1, code: "formTemplate-901").Generate();
        var formTemplate2 = new FormTemplateAggregateFaker(index: 2, code: "formTemplate-902").Generate();

        var testCollection = Enumerable
            .Range(100, 100)
            .Select(idx => new FormTemplateAggregateFaker(index: idx, code: $"formTemplate-{idx}").Generate())
            .Union(new[] { formTemplate1, formTemplate2 })
            .Reverse()
            .ToList();

        var request = new List.Request
        {
            SortColumnName = columnName,
            SortOrder = sortOrder,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListFormTemplatesSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(formTemplate1, cfg => cfg.ExcludingMissingMembers());
        result.Should().ContainEquivalentOf(formTemplate2, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public void ListFormTemplatesSpecification_AppliesSortOrderCorrectly()
    {
        // Arrange
        var formTemplate1 = new FormTemplateAggregateFaker(index: 1, code: "formTemplate-101").Generate();
        var formTemplate2 = new FormTemplateAggregateFaker(index: 2, code: "formTemplate-102").Generate();

        var testCollection = Enumerable
            .Range(900, 100)
            .Select(idx => new FormTemplateAggregateFaker(index: idx, code: $"formTemplate-{idx}").Generate())
            .Union(new[] { formTemplate1, formTemplate2 })
            .Reverse()
            .ToList();

        var request = new List.Request
        {
            SortColumnName = "code",
            SortOrder = SortOrder.Desc,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListFormTemplatesSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(formTemplate1, cfg => cfg.ExcludingMissingMembers());
        result.Should().ContainEquivalentOf(formTemplate2, cfg => cfg.ExcludingMissingMembers());
    }
    public static IEnumerable<object[]> NameSortingTestCases =>
        new List<object[]>
        {
            new object[] { "code", null },
            new object[] { "Code", null },
            new object[] { "code", SortOrder.Asc },
            new object[] { "Code", SortOrder.Asc }
        };
}
