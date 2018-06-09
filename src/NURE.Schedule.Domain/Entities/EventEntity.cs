using System;
using NURE.Schedule.Common;

namespace NURE.Schedule.Domain.Entities
{
  public class EventEntity
  {
    public long Id { get; set; }
    public long SubjectId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public EventType Type { get; set; }
    public int NumberPair { get; set; }
    public string Auditory { get; set; }
    public string Teachers { get; set; }
    public string Groups { get; set; }
  }
}