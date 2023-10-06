
namespace Vote.Monitor.Core;
public  interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> Add(TEntity entity);
    Task<TEntity> GetById(int id);
    Task<TEntity> Update(int id, TEntity entity);
    Task Delete(int id);
    Task DeleteAll();
    Task<IEnumerable<TEntity>> GetAll();

}
