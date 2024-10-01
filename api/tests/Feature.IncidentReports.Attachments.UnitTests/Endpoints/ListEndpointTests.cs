using Feature.IncidentsReports.Attachments.List;
using Feature.IncidentsReports.Attachments.Specifications;

namespace Feature.IncidentReports.Attachments.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<IncidentReportAttachment> _repository;
    private readonly Endpoint _endpoint;

    public ListEndpointTests()
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
        var anotherIncidentReportAttachment = new IncidentReportAttachmentFaker().Generate();

        _repository
            .ListAsync(Arg.Any<ListIncidentReportAttachmentsSpecification>())
            .Returns([incidentReportAttachment, anotherIncidentReportAttachment]);

        // Act
        var request = new Request
        {
            ElectionRoundId = incidentReportAttachment.ElectionRoundId,
            IncidentReportId = incidentReportAttachment.IncidentReportId,
            FormId = incidentReportAttachment.FormId,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var attachments = result.Attachments;

        attachments.Should().HaveCount(2);
        attachments.First().FileName.Should().Be(incidentReportAttachment.FileName);
        attachments.First().Id.Should().Be(incidentReportAttachment.Id);
        attachments.Last().FileName.Should().Be(anotherIncidentReportAttachment.FileName);
        attachments.Last().Id.Should().Be(anotherIncidentReportAttachment.Id);
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenAttachmentsDoNotExist()
    {
        // Arrange

        _repository
            .ListAsync(Arg.Any<ListIncidentReportAttachmentsSpecification>(), CancellationToken.None)
            .Returns([]);

        // Act
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid(),
            FormId = Guid.NewGuid(),
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var attachments = result.Attachments;
        attachments.Should().BeEmpty();
    }
}