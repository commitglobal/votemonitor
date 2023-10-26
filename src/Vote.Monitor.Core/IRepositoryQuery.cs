
namespace Vote.Monitor.Core;

public interface IRepositoryQuery<TEntity, TTag> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(List<TTag>? filterCriteria, int pageSize, int page);
    Task<IEnumerable<TEntity>> GetAllAsync(int pageSize, int page);
    Task<int> CountAsync(List<TTag>? filterCriteria);
}
