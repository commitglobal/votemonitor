using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IRepository<Form> _repository = Substitute.For<IRepository<Form>>();
    private readonly IRepository<FormSubmission> _formSubmissionsRepository = Substitute.For<IRepository<FormSubmission>>();
    private readonly IRepository<IncidentReport> _incidentReportsRepository = Substitute.For<IRepository<IncidentReport>>();
    private readonly IRepository<MonitoringNgo> _monitoringNgoRepository = Substitute.For<IRepository<MonitoringNgo>>();
    private readonly Guid _initialFormVersion = Guid.NewGuid();
    private readonly MonitoringNgo _monitoringNgo;
    private readonly Delete.Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _endpoint = Factory.Create<Delete.Endpoint>(_authorizationService, _monitoringNgoRepository, _repository,
            _formSubmissionsRepository, _incidentReportsRepository);
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());

        _monitoringNgo = new MonitoringNgoAggregateFaker(formsVersions: _initialFormVersion).Generate();
        _monitoringNgoRepository.GetByIdAsync(_monitoringNgo.Id).Returns(_monitoringNgo);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenUserIsNotAuthorized()
    {
        // Arrange
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Delete.Request();

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnConflict_When_Published_And_Form_HasSubmissions()
    {
        // Arrange
        var form = new FormAggregateFaker(status: FormStatus.Published, formType: FormType.Opening).Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .Returns(form);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(_monitoringNgo);

        _formSubmissionsRepository
            .AnyAsync(Arg.Any<GetSubmissionsForFormSpecification>())
            .Returns(true);

        // Act
        var request = new Delete.Request
        {
            NgoId = _monitoringNgo.NgoId,
            Id = form.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert

        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict>>()
            .Which
            .Result.Should().BeOfType<Conflict>();
    }
    
    [Fact]
    public async Task ShouldReturnConflict_When_Published_Type_IncidentReporting_And_HasReports()
    {
        // Arrange
        var form = new FormAggregateFaker(status: FormStatus.Published, formType: FormType.IncidentReporting)
            .Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .Returns(form);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(_monitoringNgo);

        _incidentReportsRepository
            .AnyAsync(Arg.Any<GetIncidentReportsForFormSpecification>())
            .Returns(true);

        // Act
        var request = new Delete.Request
        {
            NgoId = _monitoringNgo.NgoId,
            Id = form.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict>>()
            .Which
            .Result.Should().BeOfType<Conflict>();
    }

    [Fact]
    public async Task Should_DeleteForm_And_ReturnNoContent_WhenFormExists()
    {
        // Arrange
        var form = new FormAggregateFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .Returns(form);

        _monitoringNgoRepository
                 .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
                 .Returns(_monitoringNgo);

        // Act
        var request = new Delete.Request
        {
            NgoId = _monitoringNgo.NgoId,
            Id = form.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.Received(1).DeleteAsync(form);

        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldUpdateFormVersion_WhenValidRequest()
    {
        // Arrange
        var form = new FormAggregateFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .Returns(form);

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(_monitoringNgo);

        // Act
        var request = new Delete.Request
        {
            NgoId = _monitoringNgo.NgoId,
            Id = form.Id
        };
        await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _monitoringNgoRepository
            .Received(1)
            .UpdateAsync(Arg.Is<MonitoringNgo>(x => x.Id == _monitoringNgo.Id
                                                    && x.FormsVersion != _initialFormVersion));
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormNotFound()
    {
        // Arrange
        var request = new Delete.Request { Id = Guid.NewGuid() };

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormByIdSpecification>())
            .ReturnsNullForAnyArgs();

        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.DidNotReceiveWithAnyArgs().DeleteAsync(Arg.Any<Form>());

        result
            .Should().BeOfType<Results<NoContent, NotFound, Conflict>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
