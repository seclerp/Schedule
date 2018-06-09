using System.Collections.Generic;
using System.Threading.Tasks;
using NURE.Schedule.Domain.Entities;

namespace NURE.Schedule.Domain.Repositories.Interfaces
{
  public interface ILastUpdateRepository
  {
    Task<IEnumerable<LastUpdateEntity>> GetAllAsync();
    Task<LastUpdateEntity> GetAsync(long id);
    Task AddAsync(LastUpdateEntity entity);
    Task UpdateAsync(LastUpdateEntity entity);
    Task RemoveAsync(long id);
    Task RemoveAllAsync();
  }
}