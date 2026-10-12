namespace Feature.CitizenReports.Specifications;

public sealed class GetCitizenReportByIdReadOnlySpecification : SingleResultSpecification<CitizenReportAggregate>
{
    public GetCitizenReportByIdReadOnlySpecification(Guid electionId, Guid formId, Guid reportId,
        bool includeNotesAndAttachments = false)
    {
        Query.AsNoTracking();

        Query.Where(x => x.ElectionRoundId == electionId && x.Form.Id == formId && x.Id == reportId);

        if (includeNotesAndAttachments)
        {
            Query.Include(x => x.Attachments)
                .Include(x => x.Notes)
                .AsSplitQuery();
        }
    }
}