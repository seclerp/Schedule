using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using NURE.Schedule.Domain.Entities;
using NURE.Schedule.Domain.Repositories.Interfaces;

namespace NURE.Schedule.Domain.Repositories
{
  public class EventsRepository : IEventsRepository
  {
    private const string Table = "events";

    private IDbConnection _connection;

    public EventsRepository(IDbConnection connection)
    {
      _connection = connection;
    }

    public async Task<IEnumerable<EventEntity>> GetAllAsync(long timeTableId)
    {
      var sql = $"SELECT * FROM {Table} WHERE Id = @Id";

      return await _connection.QueryAsync<EventEntity>(sql, new { Id = timeTableId });
    }

    public async Task<IEnumerable<EventEntity>> GetAsync(long timeTableId, DateTime startTime)
    {
      var sql = $"SELECT * FROM {Table} WHERE SubjectId = @SubjectId AND StartTime = @StartTime";

      return await _connection.QueryAsync<EventEntity>(sql, new { Id = timeTableId, StartTime = startTime });
    }

    public async Task AddAsync(EventEntity entity)
    {
      var sql = $"INSERT INTO {Table} " +
                $"(Id, SubjectId, StartTime, EndTime, EventType, HourNumber, Auditory, Teachers, Groups) " +
                $"VALUES (@Id, @SubjectId, @StartTime, @EndTime, @EventType, @HourNumber, @Auditory, @Teachers, @Groups)";

      await _connection.ExecuteAsync(sql, entity);
    }

    public async Task RemoveAllAsync(long timeTableId)
    {
      var sql = $"DELETE FROM {Table} WHERE Id = @Id";

      await _connection.ExecuteAsync(sql, new { Id = timeTableId });
    }

    public async Task RemoveAllAsync()
    {
      var sql = $"DELETE FROM {Table}";

      await _connection.ExecuteAsync(sql);
    }
  }
}