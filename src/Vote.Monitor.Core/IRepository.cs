
namespace Vote.Monitor.Core;
public interface IRepositoryCommand<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> GetByIdAsync(Guid id);
    Task<TEntity> UpdateAsync(Guid id, TEntity entity);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();

}
