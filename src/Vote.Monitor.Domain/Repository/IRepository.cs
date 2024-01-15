﻿namespace Vote.Monitor.Domain.Repository;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot;
