using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

public abstract record FillInPollingStationInformationResult
{
    public record Ok(PollingStationInformation PollingStationInformation) : FillInPollingStationInformationResult;
    public record ValidationFailed(ValidationResult ValidationResult) : FillInPollingStationInformationResult;
    private FillInPollingStationInformationResult() { }
}
