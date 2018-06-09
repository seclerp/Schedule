using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NURE.Schedule.Domain.Entities;
using NURE.Schedule.Domain.Repositories.Interfaces;

namespace NURE.Schedule.Domain.Repositories
{
  public class LastUpdateRepository : ILastUpdateRepository
  {
    private const string Table = "last_updates";
    
    private IDbConnection _connection;

    public LastUpdateRepository(IDbConnection connection)
    {
      _connection = connection;
    }

    public async Task<IEnumerable<LastUpdateEntity>> GetAllAsync()
    {
      var sql = $"SELECT * FROM {Table}";
      
      return await _connection.QueryAsync<LastUpdateEntity>(sql);
    }
    
    public async Task<LastUpdateEntity> GetAsync(long id)
    {
      var sql = $"SELECT * FROM {Table} WHERE Id = @Id";
      
      return (await _connection.QueryAsync<LastUpdateEntity>(sql, new { Id = id }))
        .FirstOrDefault();
    }

    public async Task AddAsync(LastUpdateEntity entity)
    {
      var sql = $"INSERT INTO {Table} (Id, DateTime) VALUES (@Id, @DateTime)";

      await _connection.ExecuteAsync(sql, entity);
    }
    
    public async Task UpdateAsync(LastUpdateEntity entity)
    {
      var sql = $"UPDATE {Table} SET DateTime = @DateTime WHERE Id = @Id";

      await _connection.ExecuteAsync(sql, entity);
    }

    public async Task RemoveAsync(long id)
    {
      var sql = $"DELETE FROM {Table} WHERE Id = @Id";
      
      await _connection.ExecuteAsync(sql, new { Id = id });
    }
    
    public async Task RemoveAllAsync()
    {
      var sql = $"DELETE FROM {Table} WHERE TRUE";
      
      await _connection.ExecuteAsync(sql);
    }
  }
}