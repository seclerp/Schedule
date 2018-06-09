using System.Collections.Generic;
using System.Threading.Tasks;
using NURE.Schedule.Common;
using NURE.Schedule.Domain.Entities;

namespace NURE.Schedule.Domain.Repositories.Interfaces
{
  public interface ISearchItemRepository
  {
    Task<IEnumerable<SearchItemEntity>> GetAllAsync();
    Task<IEnumerable<SearchItemEntity>> GetAllFilteredAsync(string pattern, SearchItemType type);
    Task<SearchItemEntity> GetAsync(long id);
    Task AddAsync(SearchItemEntity searchItem);
    Task AddRangeAsync(IEnumerable<SearchItemEntity> entities);
    Task UpdateAsync(SearchItemEntity searchItem);
    Task RemoveAsync(long id);
    Task RemoveAllAsync();
  }
}