using Ardalis.Specification;
using Vote.Monitor.Domain.Entities;

namespace Vote.Monitor.Domain.Repository;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}