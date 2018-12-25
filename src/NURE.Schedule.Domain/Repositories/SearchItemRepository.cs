using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NURE.Schedule.Common;
using NURE.Schedule.Domain.Entities;
using NURE.Schedule.Domain.Repositories.Interfaces;

namespace NURE.Schedule.Domain.Repositories
{
  public class SearchItemRepository : ISearchItemRepository
  {
    private const string Table = "search_items";

    private IDbConnection _connection;

    public SearchItemRepository(IDbConnection connection)
    {
      _connection = connection;
    }

    public async Task<IEnumerable<SearchItemEntity>> GetAllAsync()
    {
      var sql = $"SELECT * FROM {Table}";

      return await _connection.QueryAsync<SearchItemEntity>(sql);
    }

    public async Task<IEnumerable<SearchItemEntity>> GetAllFilteredAsync(string pattern, SearchItemType type)
    {
      var sql = $"SELECT * FROM {Table} WHERE Value LIKE @Pattern OR FullValue LIKE @Pattern AND ItemType = @ItemType";

      return await _connection.QueryAsync<SearchItemEntity>(sql,
        new
        {
          Pattern = $"%{pattern}%",
          ItemType = (int) type,
        }
      );
    }

    public async Task<SearchItemEntity> GetAsync(long id)
    {
      var sql = $"SELECT * FROM {Table} WHERE Id = @Id";

      return (await _connection.QueryAsync<SearchItemEntity>(sql, new { Id = id }))
        .FirstOrDefault();
    }

    public async Task AddAsync(SearchItemEntity searchItem)
    {
      var sql = $"INSERT INTO {Table} (Id, Value, FullValue, ItemType) VALUES (@Id, @Value, @FullValue, @ItemType)";

      await _connection.ExecuteAsync(sql, searchItem);
    }

    public async Task AddRangeAsync(IEnumerable<SearchItemEntity> entities)
    {
      await _connection.ExecuteAsync("START TRANSACTION");

      foreach (var entity in entities)
      {
        await _connection.ExecuteAsync(
          $"INSERT INTO {Table} (Id, Value, FullValue, ItemType) VALUES (@Id, @Value, @FullValue, @ItemType)"
          , entity);
      }

      await _connection.ExecuteAsync("COMMIT");
    }

    public async Task UpdateAsync(SearchItemEntity searchItem)
    {
      var sql = $"UPDATE {Table} SET Value = @Value, FullValue = @FullValue, ItemType = @ItemType WHERE Id = @Id";

      await _connection.ExecuteAsync(sql, searchItem);
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