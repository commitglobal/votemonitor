using Ardalis.Specification;

namespace Feature.CitizenReports.Notes.Specifications;

public sealed class ListNotesSpecification : Specification<CitizenReportNoteAggregate, CitizenReportNoteModel>
{
    public ListNotesSpecification(Guid electionRoundId, Guid citizenReportId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId && x.FormId == citizenReportId &&
            x.Form.ElectionRoundId == electionRoundId
        );

        Query.Select(note => CitizenReportNoteModel.FromEntity(note));
    }
}
