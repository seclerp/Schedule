using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Events
{
  public class Hour
  {
    [JsonProperty("type")]
    public long Type { get; set; }

    [JsonProperty("val")]
    public long Val { get; set; }

    [JsonProperty("teachers")]
    public long[] Teachers { get; set; }
  }
}