using FastEndpoints;
using Feature.CitizenReports.Notes.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Entities.CitizenReportNoteAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.CitizenReports.Notes.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<CitizenReportNote> _repository;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<CitizenReportNote>>();
        _endpoint = Factory.Create<Get.Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
    {
        // Arrange
        var fakeNote = new CitizenReportNoteFaker().Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .Returns(fakeNote);

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = fakeNote.ElectionRoundId,
            Id = fakeNote.Id
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        var model = result.Result.As<Ok<CitizenReportNoteModel>>();
        model.Value!.Text.Should().Be(fakeNote.Text);
        model.Value.Id.Should().Be(fakeNote.Id);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotExists()
    {
        // Arrange
        _repository.FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .ReturnsNull();

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<CitizenReportNoteModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}