using Ardalis.Specification;
using Vote.Monitor.Core.Entities;
using Vote.Monitor.Domain.Entities;

namespace Vote.Monitor.Domain.Repository;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}