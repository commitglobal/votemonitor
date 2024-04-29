namespace Job.Contracts.Jobs;

public interface IExportFormSubmissionsJob
{
    Task ExportFormSubmissions(Guid electionRoundId, Guid ngoId, Guid exportedDataId);
}
