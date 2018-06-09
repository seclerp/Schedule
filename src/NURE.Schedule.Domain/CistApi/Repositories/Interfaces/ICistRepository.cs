using System.Threading.Tasks;
using NURE.Schedule.Domain.CistApi.Events;
using NURE.Schedule.Domain.CistApi.Structure;

namespace NURE.Schedule.Domain.CistApi.Repositories.Interfaces
{
  public interface ICistRepository
  {
    Task<UniversityInfo> GetTeachersUniversityInfoAsync();
    Task<UniversityInfo> GetGroupsUniversityInfoAsync();
    Task<EventsInfo> GetTeachersEventsInfo(long groupId);
    Task<EventsInfo> GetGroupEventsInfo(long teacherId);
  }
}