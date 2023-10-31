
namespace Vote.Monitor.Core;

public interface IRepositoryQuery<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(Dictionary<string, string>? filter, int pageSize, int page);
    Task<IEnumerable<TEntity>> GetAllAsync(int pageSize, int page);
    Task<int> CountAsync(Dictionary<string, string>? filter);
    Task<List<string>> GetTagKeys(Dictionary<string, string>? filter);
    Task<List<TagModel>> GetTagValuesAsync(string selectTag, Dictionary<string, string>? filter);
 }



public class TagModel
{
    public required string Key { get; set; }
    public required string Value { get; set; }

}
