using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Events
{
  public class Subject
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("brief")]
    public string Brief { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("hours")]
    public Hour[] Hours { get; set; }
  }
}