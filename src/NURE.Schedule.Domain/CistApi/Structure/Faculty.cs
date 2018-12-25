using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Structure
{
  public class Faculty
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("short_name")]
    public string ShortName { get; set; }

    [JsonProperty("full_name")]
    public string FullName { get; set; }

    [JsonProperty("departments")]
    public Department[] Departments { get; set; }

    [JsonProperty("directions")]
    public Direction[] Directions { get; set; }

    [JsonProperty("groups")]
    public Group[] Groups { get; set; }
  }
}