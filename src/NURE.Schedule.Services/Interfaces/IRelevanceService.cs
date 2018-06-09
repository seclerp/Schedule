using System.Threading.Tasks;

namespace NURE.Schedule.Services.Interfaces
{
  public interface IRelevanceService
  {
    Task<bool> IsTimeTableRelevantAsync(long timeTableId);
  }
}