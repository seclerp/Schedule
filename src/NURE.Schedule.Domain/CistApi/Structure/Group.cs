using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Structure
{
  public class Group
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
  }
}