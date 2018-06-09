using Newtonsoft.Json;
using NURE.Schedule.Domain.CistApi.Structure;

namespace NURE.Schedule.Domain.CistApi.Events
{
  public class EventsInfo
  {
    [JsonProperty("time-zone")]
    public string TimeZone { get; set; }

    [JsonProperty("events")]
    public Event[] Events { get; set; }

    [JsonProperty("groups")]
    public Group[] Groups { get; set; }

    [JsonProperty("teachers")]
    public Teacher[] Teachers { get; set; }

    [JsonProperty("subjects")]
    public Subject[] Subjects { get; set; }

    [JsonProperty("types")]
    public EventType[] Types { get; set; }
  }
}