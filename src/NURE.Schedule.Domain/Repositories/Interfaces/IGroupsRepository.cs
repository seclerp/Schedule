using System.Collections.Generic;
using System.Threading.Tasks;
using NURE.Schedule.Domain.Entities;

namespace NURE.Schedule.Domain.Repositories.Interfaces
{
  public interface IGroupsRepository
  {
    Task<IEnumerable<GroupEntity>> GetAllAsync();
    Task<GroupEntity> GetAsync(long id);
    Task AddAsync(GroupEntity entity);
    Task UpdateAsync(GroupEntity entity);
    Task RemoveAsync(long id);
    Task RemoveAllAsync();
  }
}