using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NURE.Schedule.Domain.Entities;
using NURE.Schedule.Domain.Repositories.Interfaces;

namespace NURE.Schedule.Domain.Repositories
{
  public class GroupsRepository : IGroupsRepository
  {
    private const string Table = "groups";
    
    private IDbConnection _connection;

    public GroupsRepository(IDbConnection connection)
    {
      _connection = connection;
    }

    public async Task<IEnumerable<GroupEntity>> GetAllAsync()
    {
      var sql = $"SELECT * FROM {Table}";
      
      return await _connection.QueryAsync<GroupEntity>(sql);
    }
    
    public async Task<GroupEntity> GetAsync(long id)
    {
      var sql = $"SELECT * FROM {Table} WHERE Id = @Id";
      
      return (await _connection.QueryAsync<GroupEntity>(sql, new { Id = id }))
        .Single();
    }

    public async Task AddAsync(GroupEntity entity)
    {
      var sql = $"INSERT INTO {Table} (Id, Name) VALUES (@Id, @Name)";

      await _connection.ExecuteAsync(sql, entity);
    }
    
    public async Task UpdateAsync(GroupEntity entity)
    {
      var sql = $"UPDATE {Table} SET Name = @Name WHERE Id = @Id";

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