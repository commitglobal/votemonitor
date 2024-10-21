namespace Feature.CitizenReports.Specifications;

public sealed class GetCitizenReportByIdSpecification : SingleResultSpecification<CitizenReportAggregate>
{
    public GetCitizenReportByIdSpecification(Guid electionId, Guid formId, Guid reportId,
        bool includeNotesAndAttachments = false)
    {
        Query.Where(x => x.ElectionRoundId == electionId && x.Form.Id == formId && x.Id == reportId);

        if (includeNotesAndAttachments)
        {
            Query.Include(x => x.Attachments)
                .Include(x => x.Notes)
                .AsSplitQuery();
        }
    }
}