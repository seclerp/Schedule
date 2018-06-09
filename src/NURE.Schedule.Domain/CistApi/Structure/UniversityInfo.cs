using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Structure
{
  public class UniversityInfo
  {   
    [JsonProperty("short_name")]
    public string ShortName { get; set; }

    [JsonProperty("full_name")]
    public string FullName { get; set; }
    
    [JsonProperty("university")]
    public University University { get; set; }
  }
}