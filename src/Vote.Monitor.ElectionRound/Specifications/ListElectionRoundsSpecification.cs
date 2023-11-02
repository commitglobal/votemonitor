using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Vote.Monitor.ElectionRound.Specifications;

public class ListElectionRoundsSpecification : Specification<Domain.Entities.ElectionRoundAggregate.ElectionRound>
{
    public ListElectionRoundsSpecification(string nameFilter, ElectionRoundStatus? electionRoundStatus, int pageSize,
        int page)
    {

        Query
            .Search(x => x.Name, nameFilter, !string.IsNullOrEmpty(nameFilter))
            .Where(x => x.Status == electionRoundStatus, electionRoundStatus != null)
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
