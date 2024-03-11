//namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Specifications;

//public class GetObserverStatusSpecificationTests
//{
//    [Theory]
//    [MemberData(nameof(ObserverStatuses))]
//    public void ShouldMatch_ObserverById(UserStatus status)
//    {
//        // Arrange
//        var observeId = Guid.NewGuid();
//        var observer = new ObserverAggregateFaker(observeId, status: status).Generate();

//        List<ObserverAggregate> testCollection =
//        [
//            observer,
//            .. new ObserverAggregateFaker().Generate(100)
//        ];

//        // Act
//        var spec = new GetObserverStatusSpecification(observeId);
//        var result = spec.Evaluate(testCollection).ToList();

//        // Assert
//        result.Should().HaveCount(1);
//        result.Should().Contain(status);
//    }

//    [Fact]
//    public void ShouldNotMatchAny_WhenNoSuchObserver()
//    {
//        // Arrange
//        var observeId = Guid.NewGuid();

//        List<ObserverAggregate> testCollection =
//        [
//            .. new ObserverAggregateFaker().Generate(100)
//        ];

//        // Act
//        var spec = new GetObserverStatusSpecification(observeId);
//        var result = spec.Evaluate(testCollection).FirstOrDefault();

//        // Assert
//        result.Should().BeNull();
//    }

//    public static IEnumerable<object[]> ObserverStatuses =>
//        new List<object[]>
//        {
//            new object[] { UserStatus.Deactivated },
//            new object[] { UserStatus.Active }
//        };
//}
