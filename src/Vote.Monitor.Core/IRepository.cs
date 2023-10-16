
namespace Vote.Monitor.Core;
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> GetByIdAsync(int id);
    Task<TEntity> UpdateAsync(int id, TEntity entity);
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
    Task<IEnumerable<TEntity>> GetAllAsync(Dictionary<string, string>? filterCriteria, int pageSize, int page);
    Task<IEnumerable<TEntity>> GetAllAsync(int pageSize, int page);
    Task<int> CountAsync(Dictionary<string, string>? filterCriteria);
}
