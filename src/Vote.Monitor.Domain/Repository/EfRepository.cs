using Ardalis.Specification.EntityFrameworkCore;
using Vote.Monitor.Core.Entities;
using Vote.Monitor.Domain.Entities;

namespace Vote.Monitor.Domain.Repository;

public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(VoteMonitorContext dbContext) : base(dbContext)
    {
    }
}
