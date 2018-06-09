using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NURE.Schedule.Api.Models.Request;
using NURE.Schedule.Services.Interfaces;
using NURE.Schedule.Services.Models;

namespace NURE.Schedule.Api.Controllers
{
  [Route("schedules")]
  public class SearchSchedulesController : Controller
  {
    private readonly ISearchService _service;

    public SearchSchedulesController(ISearchService service)
    {
      _service = service;
    }
    
    public async Task<IEnumerable<SearchResultModel>> GetAsync(
      [FromQuery] SearchScheduleRequestModel model
    )
    {
      return await _service.SearchAsync(model.Search?.Trim(), model.Teachers, model.Groups);
    }
  }
}