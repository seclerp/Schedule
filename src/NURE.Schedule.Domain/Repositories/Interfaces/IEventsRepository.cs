using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NURE.Schedule.Domain.Entities;

namespace NURE.Schedule.Domain.Repositories.Interfaces
{
  public interface IEventsRepository
  {
    Task<IEnumerable<EventEntity>> GetAllAsync(long timeTableId);
    Task<IEnumerable<EventEntity>> GetAsync(long timeTableId, DateTime startTime);
    Task AddAsync(EventEntity entity);
    Task RemoveAllAsync(long timeTableId);
    Task RemoveAllAsync();
  }
}