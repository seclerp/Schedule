namespace NURE.Schedule.Api.Models.Request
{
  public class SearchScheduleRequestModel
  {
    public string Search { get; set; }
    public bool Teachers { get; set; }
    public bool Groups { get; set; }
  }
}