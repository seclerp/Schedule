using System.Threading.Tasks;
using NURE.Schedule.Common;
using NURE.Schedule.Domain.CistApi.Events;
using NURE.Schedule.Domain.CistApi.Repositories.Interfaces;
using NURE.Schedule.Domain.CistApi.Structure;

namespace NURE.Schedule.Domain.CistApi.Repositories
{
  public class CistRepository : ICistRepository
  {
    private const string CistApiRoot             = "http://cist.nure.ua/ias/app/tt/";
    private const string GetTeachersInfoMethod   = "P_API_PODR_JSON";
    private const string GetGroupsInfoMethod     = "P_API_GROUP_JSON";
    private const string GetGroupEventsMethod    = "P_API_EVENTS_GROUP_JSON";
    private const string GetTeachersEventsMethod = "P_API_EVENTS_TEACHER_JSON";
    
    private WebClient _client;

    public CistRepository()
    {
      _client = new WebClient();
    }
    
    public async Task<UniversityInfo> GetTeachersUniversityInfoAsync()
    {
      return await _client.GetAsync<UniversityInfo>($"{CistApiRoot}{GetTeachersInfoMethod}");
    }
    
    public async Task<UniversityInfo> GetGroupsUniversityInfoAsync()
    {
      return await _client.GetAsync<UniversityInfo>($"{CistApiRoot}{GetGroupsInfoMethod}");
    }

    public async Task<EventsInfo> GetTeachersEventsInfo(long groupId)
    {
      return await _client.GetAsync<EventsInfo>($"{CistApiRoot}{GetGroupEventsMethod}", 
        ("p_id_group", groupId.ToString()));
    }
    
    public async Task<EventsInfo> GetGroupEventsInfo(long teacherId)
    {
      return await _client.GetAsync<EventsInfo>($"{CistApiRoot}{GetTeachersEventsMethod}", 
        ("p_id_teacher", teacherId.ToString()));
    }
  }
}