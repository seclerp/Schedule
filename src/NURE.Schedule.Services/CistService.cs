using System.Collections.Generic;
using System.Threading.Tasks;
using NURE.Schedule.Domain.CistApi.Repositories.Interfaces;
using NURE.Schedule.Domain.CistApi.Structure;
using NURE.Schedule.Services.Interfaces;

namespace NURE.Schedule.Services
{
  public class CistService : ICistService
  {
    private ICistRepository _repository;

    public CistService(ICistRepository repository)
    {
      _repository = repository;
    }

    public async Task<IEnumerable<Teacher>> GetTeachersAsync()
    {
      var result = new List<Teacher>();
      
      var newTeachersInfo = await _repository.GetTeachersUniversityInfoAsync();

      foreach (var faculty in newTeachersInfo.University.Faculties)
      {
        foreach (var department in faculty.Departments)
        {
          foreach (var teacher in department.Teachers)
          {
            if (!result.Exists(x => x.Id == teacher.Id))
            {
              result.Add(teacher);
            }
          }
        }
      }

      return result;
    }
    
    public async Task<IEnumerable<Group>> GetGroupsAsync()
    {
      var result = new List<Group>();
      
      var newTeachersInfo = await _repository.GetGroupsUniversityInfoAsync();

      foreach (var faculty in newTeachersInfo.University.Faculties)
      {
        foreach (var direction in faculty.Directions)
        {
          if (!(direction.Groups is null))
          {
            foreach (var group in direction.Groups)
            {
              if (!result.Exists(x => x.Id == group.Id))
              {
                result.Add(group);
              }
            }
          }
          
          if (!(direction.Specialities is null))
          {
            foreach (var speciality in direction.Specialities)
            {
              foreach (var group in speciality.Groups)
              {
                if (!result.Exists(x => x.Id == group.Id))
                {
                  result.Add(group);
                }
              }
            }
          }
        }
      }
      
      return result;
    }
  }
}