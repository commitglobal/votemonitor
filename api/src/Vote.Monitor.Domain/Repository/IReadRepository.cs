namespace Vote.Monitor.Domain.Repository;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot;
