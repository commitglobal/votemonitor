using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;

internal class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        //temp 
        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

    public async Task<int> CountAsync(List<TagModel>? filterCriteria)
    {
        //TODO : implement filterCriteria
        return await _context.Tags.CountAsync();

        //if (filterCriteria == null || filterCriteria.Count == 0) return await _context.Tags.CountAsync();

        //return _context.PollingStations.AsEnumerable().Where(
        //    station => filterCriteria.Count(filter => filterCriteria.All(tag => station.Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))) == filterCriteria.Count
        //      ).Count();
    }

    public async Task<IEnumerable<TagModel>> GetAllAsync(int pageSize=0, int page=1)
    {
        return await _context.Tags
            .Select(tag => new TagModel { Id = tag.Id, Key = tag.Key, Value = tag.Value })
            .ToListAsync();
           
    }


    public async Task<IEnumerable<TagModel>> GetAllAsync(List<TagModel>? filterCriteria, int pageSize = 0, int page = 1)
    {
        if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (pageSize > 0 && page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        return await GetAllAsync(pageSize, page);

        //TODO: implement filterCriteria
        //if (filterCriteria == null || filterCriteria.Count == 0)  return await GetAllAsync(pageSize, page);

        //if (pageSize == 0) return _context.PollingStations.AsEnumerable().Where(
        //    station => filterCriteria.Count(filter => filterCriteria.All(tag => station.Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))) == filterCriteria.Count
        //      ).OrderBy(st => st.DisplayOrder);

        //return _context.PollingStations.AsEnumerable().Where(
        //    station => filterCriteria.Count(filter => filterCriteria.All(tag => station.Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))) == filterCriteria.Count
        //      ).OrderBy(st => st.DisplayOrder)
        //    .Skip((page - 1) * pageSize)
        //    .Take(pageSize);
    }
}
