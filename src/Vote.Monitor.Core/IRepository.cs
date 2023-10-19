
namespace Vote.Monitor.Core;
public interface IRepositoryCommand<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> GetByIdAsync(int id);
    Task<TEntity> UpdateAsync(int id, TEntity entity);
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
 
}
public interface IRepositoryQuery<TEntity,TTag> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(List<TTag>? filterCriteria, int pageSize, int page);
    Task<IEnumerable<TEntity>> GetAllAsync(int pageSize, int page);
    Task<int> CountAsync(List<TTag>? filterCriteria);
}
