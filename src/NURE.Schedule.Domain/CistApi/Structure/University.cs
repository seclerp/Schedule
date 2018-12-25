using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Structure
{
  public class University
  {
    [JsonProperty("short_name")]
    public string ShortName { get; set; }

    [JsonProperty("full_name")]
    public string FullName { get; set; }

    [JsonProperty("faculties")]
    public Faculty[] Faculties { get; set; }
  }
}