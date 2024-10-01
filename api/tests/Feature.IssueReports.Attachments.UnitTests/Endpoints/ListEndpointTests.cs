namespace Feature.IssueReports.Attachments.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<IssueReportAttachment> _repository;
    private readonly List.Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<IssueReportAttachment>>();
        var fileStorageService = Substitute.For<IFileStorageService>();
        _endpoint = Factory.Create<List.Endpoint>(_repository, fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenAllIdsExist()
    {
        // Arrange
        var issueReportAttachment = new IssueReportAttachmentFaker().Generate();
        var anotherIssueReportAttachment = new IssueReportAttachmentFaker().Generate();

        _repository
            .ListAsync(Arg.Any<ListIssueReportAttachmentsSpecification>())
            .Returns([issueReportAttachment, anotherIssueReportAttachment]);

        // Act
        var request = new List.Request
        {
            ElectionRoundId = issueReportAttachment.ElectionRoundId,
            IssueReportId = issueReportAttachment.IssueReportId,
            FormId = issueReportAttachment.FormId,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var attachments = result.Attachments;

        attachments.Should().HaveCount(2);
        attachments.First().FileName.Should().Be(issueReportAttachment.FileName);
        attachments.First().Id.Should().Be(issueReportAttachment.Id);
        attachments.Last().FileName.Should().Be(anotherIssueReportAttachment.FileName);
        attachments.Last().Id.Should().Be(anotherIssueReportAttachment.Id);
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenAttachmentsDoNotExist()
    {
        // Arrange

        _repository
            .ListAsync(Arg.Any<ListIssueReportAttachmentsSpecification>(), CancellationToken.None)
            .Returns([]);

        // Act
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IssueReportId = Guid.NewGuid(),
            FormId = Guid.NewGuid(),
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var attachments = result.Attachments;
        attachments.Should().BeEmpty();
    }
}