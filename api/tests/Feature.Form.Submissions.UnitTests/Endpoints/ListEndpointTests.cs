using Feature.Form.Submissions.ListEntries;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<FormSubmission> _repository;
    private readonly Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<FormSubmission>>();
        _endpoint = Factory.Create<Endpoint>(_repository);
    }

}
