using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NURE.Schedule.Domain.Entities;
using NURE.Schedule.Domain.Repositories.Interfaces;

namespace NURE.Schedule.Domain.Repositories
{
  public class TeachersRepository : ITeachersRepository
  {
    private const string Table = "teachers";
    
    private IDbConnection _connection;

    public TeachersRepository(IDbConnection connection)
    {
      _connection = connection;
    }

    public async Task<IEnumerable<TeacherEntity>> GetAllAsync()
    {
      var sql = $"SELECT * FROM {Table}";
      
      return await _connection.QueryAsync<TeacherEntity>(sql);
    }
    
    public async Task<TeacherEntity> GetAsync(long id)
    {
      var sql = $"SELECT * FROM {Table} WHERE Id = @Id";
      
      return (await _connection.QueryAsync<TeacherEntity>(sql, new { Id = id }))
        .Single();
    }

    public async Task AddAsync(TeacherEntity teacher)
    {
      var sql = $"INSERT INTO {Table} (Id, ShortName, FullName) VALUES (@Id, @ShortName, @FullName)";

      await _connection.ExecuteAsync(sql, teacher);
    }
    
    public async Task UpdateAsync(TeacherEntity teacher)
    {
      var sql = $"UPDATE {Table} SET ShortName = @ShortName, FullName = @FullName WHERE Id = @Id";

      await _connection.ExecuteAsync(sql, teacher);
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