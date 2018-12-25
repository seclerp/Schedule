using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Structure
{
  public class Teacher
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("short_name")]
    public string ShortName { get; set; }

    [JsonProperty("full_name")]
    public string FullName { get; set; }
  }
}