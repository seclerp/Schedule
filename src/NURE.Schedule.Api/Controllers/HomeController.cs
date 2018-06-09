using Microsoft.AspNetCore.Mvc;
using NURE.Schedule.Api.Models.Response;

namespace NURE.Schedule.Api.Controllers
{
  [Route("")]
  public class HomeController : Controller
  {
    public string Get()
    {
      return "NURE Schedule API is running";
    }
  }
}