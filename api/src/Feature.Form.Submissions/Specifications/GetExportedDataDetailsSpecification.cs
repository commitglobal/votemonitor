using Ardalis.Specification;
using Feature.Form.Submissions.GetExportedDataDetails;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetExportedDataDetailsSpecification : SingleResultSpecification<ExportedData, ExportedDataDetails>
{
    public GetExportedDataDetailsSpecification(Guid electionRoundId, Guid ngoId, Guid exportedDataId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.NgoId == ngoId && x.Id == exportedDataId);

        Query.Select(x => new ExportedDataDetails
        {
            ExportedDataId = x.Id,
            CompletedAt = x.CompletedAt,
            ExportStatus = x.ExportStatus,
            FileName = x.FileName,
            StartedAt = x.StartedAt
        });
    }
}
