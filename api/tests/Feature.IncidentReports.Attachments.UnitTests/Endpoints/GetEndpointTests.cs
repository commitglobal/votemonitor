using Feature.IncidentsReports.Attachments;
using Feature.IncidentsReports.Attachments.Get;
using Feature.IncidentsReports.Attachments.Specifications;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute.ReturnsExtensions;

namespace Feature.IncidentReports.Attachments.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<IncidentReportAttachment> _repository;
    private readonly Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<IncidentReportAttachment>>();
        var fileStorageService = Substitute.For<IFileStorageService>();
        _endpoint = Factory.Create<Endpoint>(_repository, fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenAllIdsExist()
    {
        // Arrange
        var incidentReportAttachment = new IncidentReportAttachmentFaker().Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetAttachmentByIdSpecification>())
            .Returns(incidentReportAttachment);

        // Act
        var request = new Request
        {
            ElectionRoundId = incidentReportAttachment.ElectionRoundId,
            IncidentReportId = incidentReportAttachment.IncidentReportId,
            Id = incidentReportAttachment.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<IncidentReportAttachmentModel>>();
        model.Value!.FileName.Should().Be(incidentReportAttachment.FileName);
        model.Value.Id.Should().Be(incidentReportAttachment.Id);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenAttachmentDoesNotExist()
    {
        // Arrange
        _repository.FirstOrDefaultAsync(Arg.Any<GetAttachmentByIdSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<IncidentReportAttachmentModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}