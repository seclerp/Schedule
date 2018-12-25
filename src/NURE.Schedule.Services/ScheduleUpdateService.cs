using System.Threading.Tasks;
using NURE.Schedule.Services.Interfaces;

namespace NURE.Schedule.Services
{
  public class ScheduleUpdateService
  {
    private IRelevanceService _relevanceService;

    public ScheduleUpdateService(IRelevanceService relevanceService)
    {
      _relevanceService = relevanceService;
    }

    // -1 in 'last_updates' contains time of updating of 'teachers' table
    // -2 in 'last_updates' contains time of updating of 'groups' table


  }
}