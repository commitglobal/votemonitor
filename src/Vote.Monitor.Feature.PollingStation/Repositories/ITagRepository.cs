using Vote.Monitor.Core;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;

internal interface ITagRepository 
{
    Task<IEnumerable<TagModel>> GetTagsAsync(string selectTage, List<TagModel>? filterCriteria);
    Task<IEnumerable<string>> GetAllTagKeysAsync();
  
}
