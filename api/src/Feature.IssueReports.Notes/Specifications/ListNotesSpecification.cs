using Ardalis.Specification;

namespace Feature.IssueReports.Notes.Specifications;

public sealed class ListNotesSpecification : Specification<IssueReportNoteAggregate, IssueReportNoteModel>
{
    public ListNotesSpecification(Guid electionRoundId, Guid issueReportId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.FormId == issueReportId);
        
        Query.Select(note => IssueReportNoteModel.FromEntity(note));
    }
}