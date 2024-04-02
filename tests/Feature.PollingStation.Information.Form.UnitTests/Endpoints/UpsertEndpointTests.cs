using FastEndpoints;
using Feature.PollingStation.Information.Form.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Xunit;

namespace Feature.PollingStation.Information.Form.UnitTests.Endpoints;

public class UpsertEndpointTests
{
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationInformationForm> _formRepository;

    private readonly Upsert.Endpoint _endpoint;

    public UpsertEndpointTests()
    {
        _electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        _formRepository = Substitute.For<IRepository<PollingStationInformationForm>>();

        _endpoint = Factory.Create<Upsert.Endpoint>(_formRepository, _electionRoundRepository);
    }

    [Fact]
    public async Task ShouldUpdatePollingStationInformationForm_WhenPollingStationInformationFormExists()
    {
        // Arrange
        var pollingStationInformationForm = new PollingStationInformationFormFaker().Generate();
        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationFormSpecification>())
            .Returns(pollingStationInformationForm);

        // Act
        List<string> languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1];
        var request = new Upsert.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Languages = languages,
            Questions = [
                new NumberQuestionRequest
                {
                    Id = Guid.NewGuid(),
                    Text = new TranslatedStringFaker(languages).Generate(),
                    Code = "c1"
                }
            ]
        };

        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _formRepository.Received(1).UpdateAsync(pollingStationInformationForm);

        result
            .Should().BeOfType<Results<Ok<PollingStationInformationFormModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<PollingStationInformationFormModel>>()
            .Which.Value.Should().BeEquivalentTo(pollingStationInformationForm, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotExists()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();

        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationFormSpecification>())
            .ReturnsNull();

        var request = new Upsert.Request
        {
            ElectionRoundId = electionRoundId,
        };

        _electionRoundRepository
            .GetByIdAsync(electionRoundId)
            .ReturnsNull();

        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PollingStationInformationFormModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldCreatePollingStationInformation_WhenPollingStationInformationNotExists()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var electionRound = new ElectionRoundAggregateFaker(electionRoundId).Generate();

        _formRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationFormSpecification>())
            .ReturnsNull();

        List<string> languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1];
        var request = new Upsert.Request
        {
            ElectionRoundId = electionRoundId,
            Languages = languages,
            Questions = [
                new NumberQuestionRequest
                {
                    Id = Guid.NewGuid(),
                    Text = new TranslatedStringFaker(languages).Generate(),
                    Code = "c1"
                }
            ]
        };

        _electionRoundRepository
            .GetByIdAsync(electionRoundId)
            .Returns(electionRound);

        // Act
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _formRepository
             .Received(1)
             .AddAsync(Arg.Is<PollingStationInformationForm>(x => x.ElectionRoundId == electionRoundId));

        result
            .Should().BeOfType<Results<Ok<PollingStationInformationFormModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<PollingStationInformationFormModel>>();
    }
}
