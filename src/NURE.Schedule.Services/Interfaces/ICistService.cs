using System.Collections.Generic;
using System.Threading.Tasks;
using NURE.Schedule.Domain.CistApi.Structure;

namespace NURE.Schedule.Services.Interfaces
{
  public interface ICistService
  {
    Task<IEnumerable<Teacher>> GetTeachersAsync();
    Task<IEnumerable<Group>> GetGroupsAsync();
  }
}