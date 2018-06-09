using System.Collections.Generic;
using NURE.Schedule.Common;

namespace NURE.Schedule.Api.Models.Response
{
  public class StaticDataResponseModel
  {
    public Dictionary<EventType, string> EventTypesColors { get; set; }
  }
}