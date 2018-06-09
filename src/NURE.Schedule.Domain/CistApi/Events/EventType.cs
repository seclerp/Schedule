using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Events
{
  public class EventType
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("short_name")]
    public string ShortName { get; set; }

    [JsonProperty("full_name")]
    public string FullName { get; set; }

    [JsonProperty("id_base")]
    public long IdBase { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
  }
}