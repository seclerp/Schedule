using System.Collections.Generic;
using System.Threading.Tasks;
using NURE.Schedule.Domain.Entities;

namespace NURE.Schedule.Domain.Repositories.Interfaces
{
  public interface ITeachersRepository
  {
    Task<IEnumerable<TeacherEntity>> GetAllAsync();
    Task<TeacherEntity> GetAsync(long id);
    Task AddAsync(TeacherEntity teacher);
    Task UpdateAsync(TeacherEntity teacher);
    Task RemoveAsync(long id);
    Task RemoveAllAsync();
  }
}