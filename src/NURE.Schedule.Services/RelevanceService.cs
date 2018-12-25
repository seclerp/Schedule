using System;
using System.Threading.Tasks;
using NURE.Schedule.Domain.Repositories.Interfaces;
using NURE.Schedule.Services.Interfaces;

namespace NURE.Schedule.Services
{
  public class RelevanceService : IRelevanceService
  {
    private ILastUpdateRepository _repository;

    public RelevanceService(ILastUpdateRepository repository)
    {
      _repository = repository;
    }

    public async Task<bool> IsTimeTableRelevantAsync(long timeTableId)
    {
      var lastTeachersUpdateEntity = await _repository.GetAsync(timeTableId);

      if (lastTeachersUpdateEntity is null)
      {
        return false;
      }

      return !IsNeedUpdate(lastTeachersUpdateEntity.DateTime);
    }

    private bool IsNeedUpdate(DateTime timeOfLastUpdate)
    {
      // Cist API update schedule every day at 5:00 AM
      var nowUnix = GetUnixTime(DateTime.Now);
      var lastCistUpdateUnix = GetUnixTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 5, 0, 0));
      var lastUpdateTimeUnix = GetUnixTime(timeOfLastUpdate);

      if (nowUnix < lastCistUpdateUnix)
      {
        // Yesterday
        lastCistUpdateUnix -= 60 * 60 * 24;
      }

      var diff = lastUpdateTimeUnix - lastCistUpdateUnix;

      // If last update time less than lastCistUpdateUnix, and now time is bigger than 5:00
      return diff < 0;
    }

    private long GetUnixTime(DateTime dateTime)
    {
      return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }
  }
}