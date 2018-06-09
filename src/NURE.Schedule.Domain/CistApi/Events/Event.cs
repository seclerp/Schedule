using Newtonsoft.Json;

namespace NURE.Schedule.Domain.CistApi.Events
{
  public class Event
  {
    [JsonProperty("subject_id")]
    public long SubjectId { get; set; }

    [JsonProperty("start_time")]
    public long StartTime { get; set; }

    [JsonProperty("end_time")]
    public long EndTime { get; set; }

    [JsonProperty("type")]
    public long Type { get; set; }

    [JsonProperty("number_pair")]
    public long NumberPair { get; set; }

    [JsonProperty("auditory")]
    public string Auditory { get; set; }

    [JsonProperty("teachers")]
    public long[] Teachers { get; set; }

    [JsonProperty("groups")]
    public long[] Groups { get; set; }
  }
}