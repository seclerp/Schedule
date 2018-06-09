﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NURE.Schedule.Api.Models.Response;
using NURE.Schedule.Api.StaticData;

namespace NURE.Schedule.Api.Controllers
{
  [Route("staticdata")]
  public class StaticDataController : Controller
  {
    public StaticDataResponseModel Get()
    {
      return new StaticDataResponseModel
      {
        EventTypeColors = EventTypeStaticData.Colors,
        EventTypeNames = EventTypeStaticData.Names
      };
    }
  }
}