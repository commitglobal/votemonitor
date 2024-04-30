//using System.Security.Claims;
//using Microsoft.AspNetCore.Authorization;
//using NSubstitute.ReturnsExtensions;
//using Vote.Monitor.Domain.Entities.FormAggregate;
//using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

//namespace Feature.QuickReports.UnitTests.Endpoints;

//public class PublishEndpointTests
//{
//    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
//    private readonly IRepository<Form> _repository = Substitute.For<IRepository<Form>>();
//    private readonly IRepository<MonitoringNgo> _monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgo>>();
//    private readonly Guid _initialFormVersion = Guid.NewGuid();
//    private readonly MonitoringNgo _monitoringNgo;
//    private readonly Publish.Endpoint _endpoint;

//    public PublishEndpointTests()
//    {
//        _endpoint = Factory.Create<Publish.Endpoint>(_authorizationService, _monitoringNgoRepository, _repository);
//        _authorizationService
//            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
//                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

//        _monitoringNgo = new MonitoringNgoAggregateFaker(formsVersions: _initialFormVersion).Generate();
//        _monitoringNgoRepository.GetByIdAsync(_monitoringNgo.Id).Returns(_monitoringNgo);
//    }

//    [Fact]
//    public async Task ShouldReturnNotFound_WhenUserIsNotAuthorized()
//    {
//        // Arrange
//        _authorizationService
//            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
//            .Returns(AuthorizationResult.Failed());

//        // Act
//        var request = new Publish.Request();

//        var result = await _endpoint.ExecuteAsync(request, default);

//        // Assert
//        result
//            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
//            .Which
//            .Result.Should().BeOfType<NotFound>();
//    }

//    [Fact]
//    public async Task Should_PublishForm_And_Return_NoContent_WhenFormExists()
//    {
//        // Arrange
//        var form = new FormAggregateFaker(status: FormStatus.Published).Generate();
//        _repository
//            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
//            .Returns(form);

//        // Act
//        var request = new Publish.Request
//        {
//            MonitoringNgoId = _monitoringNgo.Id,
//            Id = form.Id
//        };
//        var result = await _endpoint.ExecuteAsync(request, default);

//        // Assert
//        await _repository
//            .Received(1)
//            .UpdateAsync(Arg.Is<Form>(x => x.Status == FormStatus.Published));

//        result
//            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
//            .Which
//            .Result.Should().BeOfType<NoContent>();
//    }

//    [Fact]
//    public async Task ShouldUpdateFormVersion_WhenValidRequest()
//    {
//        // Arrange
//        var form = new FormAggregateFaker().Generate();

//        _repository
//            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
//            .Returns(form);

//        // Act
//        var request = new Publish.Request
//        {
//            MonitoringNgoId = _monitoringNgo.Id,
//            Id = form.Id
//        };
//        await _endpoint.ExecuteAsync(request, default);

//        // Assert
//        await _monitoringNgoRepository
//            .Received(1)
//            .UpdateAsync(Arg.Is<MonitoringNgo>(x => x.Id == _monitoringNgo.Id
//                                                    && x.FormsVersion != _initialFormVersion));
//    }

//    [Fact]
//    public async Task ShouldReturnNotFound_WhenFormDoesNotExists()
//    {
//        // Arrange
//        _repository
//            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
//            .ReturnsNullForAnyArgs();

//        var request = new Publish.Request { Id = Guid.NewGuid() };

//        // Act
//        var result = await _endpoint.ExecuteAsync(request, default);

//        // Assert
//        result
//            .Should().BeOfType<Results<NoContent, NotFound, ProblemDetails>>()
//            .Which
//            .Result.Should().BeOfType<NotFound>();
//    }
//}
