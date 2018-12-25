using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NURE.Schedule.Common
{
  public class WebClient
  {
    private HttpClient _client;

    public WebClient()
    {
      _client = new HttpClient();
    }

    public async Task<string> GetStringAsync(string url, params (string Key, string Value)[] queryParansKeyValues)
    {
      var query = MakeQueryString(queryParansKeyValues);

      return await _client.GetStringAsync($"{url}{query}");
    }

    public async Task<T> GetAsync<T>(string url, params (string Key, string Value)[] queryParansKeyValues)
    {
      var @string = await GetStringAsync(url, queryParansKeyValues);

      return JsonConvert.DeserializeObject<T>(@string);
    }

    private string MakeQueryString(ICollection<(string Key, string Value)> queryParansKeyValues)
    {
      if (queryParansKeyValues is null || queryParansKeyValues.Count == 0)
      {
        return "";
      }

      var sb = new StringBuilder("?");

      foreach (var kv in queryParansKeyValues)
      {
        sb.Append($"{kv.Key}={kv.Value}&");
      }

      var result = sb.ToString();

      return result.Substring(0, result.Length - 2);
    }
  }
}