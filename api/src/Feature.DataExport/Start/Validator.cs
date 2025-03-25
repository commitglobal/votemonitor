using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Feature.DataExport.Start;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        When(x => x.ExportedDataType == ExportedDataType.FormSubmissions, () =>
        {
            RuleFor(x => x.ElectionRoundId).NotEmpty();
        });

        When(x => x.ExportedDataType == ExportedDataType.QuickReports, () =>
        {
            RuleFor(x => x.ElectionRoundId).NotEmpty();
        });

        When(x => x.ExportedDataType == ExportedDataType.CitizenReports, () =>
        {
            RuleFor(x => x.ElectionRoundId).NotEmpty();
        });

        When(x => x.ExportedDataType == ExportedDataType.IncidentReports, () =>
        {
            RuleFor(x => x.ElectionRoundId).NotEmpty();
        });

        When(x => x.ExportedDataType == ExportedDataType.PollingStations, () =>
        {
            RuleFor(x => x.ElectionRoundId).NotEmpty();
        });

        When(x => x.ExportedDataType == ExportedDataType.Locations, () =>
        {
            RuleFor(x => x.ElectionRoundId).NotEmpty();
        });
    }
}
