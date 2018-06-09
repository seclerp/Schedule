using System.Collections.Generic;
using System.Threading.Tasks;
using NURE.Schedule.Services.Models;

namespace NURE.Schedule.Services.Interfaces
{
  public interface ISearchService
  {
    Task<IEnumerable<SearchResultModel>> SearchAsync(string pattern, bool searchInTeachers, bool searchInGroups);
  }
}