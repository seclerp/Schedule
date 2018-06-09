using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Structure
{
  public class Direction
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("short_name")]
    public string ShortName { get; set; }

    [JsonProperty("full_name")]
    public string FullName { get; set; }

    [JsonProperty("specialities")]
    public Speciality[] Specialities { get; set; }

    [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
    public Group[] Groups { get; set; }
  }
}