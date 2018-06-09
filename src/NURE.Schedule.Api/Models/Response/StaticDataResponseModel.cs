using System.Collections.Generic;
using NURE.Schedule.Common;

namespace NURE.Schedule.Api.Models.Response
{
  public class StaticDataResponseModel
  {
    public Dictionary<EventType, string> EventTypeColors { get; set; }
    public Dictionary<EventType, string> EventTypeNames { get; set; }
  }
}