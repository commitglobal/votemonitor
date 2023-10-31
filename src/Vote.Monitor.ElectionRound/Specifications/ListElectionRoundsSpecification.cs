using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Vote.Monitor.ElectionRound.Specifications;

public class ListElectionRoundsSpecification : Specification<Domain.Entities.ElectionRoundAggregate.ElectionRound>
{
    public ListElectionRoundsSpecification(string nameFilter, ElectionRoundStatus? electionRoundStatus, int pageSize, int page)
    {
        if (!string.IsNullOrEmpty(nameFilter))
        {
            Query
                .Where(x => EF.Functions.Like(x.Name, $"%{nameFilter}%"));
        }

        if (electionRoundStatus != null)
        {
            Query
                .Where(x => x.Status == electionRoundStatus);
        }

        Query
            .Skip(PaginationHelper.CalculateSkip(pageSize, page))
            .Take(PaginationHelper.CalculateTake(pageSize));
    }
}
