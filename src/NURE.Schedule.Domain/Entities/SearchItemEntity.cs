using NURE.Schedule.Common;

namespace NURE.Schedule.Domain.Entities
{
  public class SearchItemEntity
  {
    public long Id { get; set; }
    public string Value { get; set; }
    public string FullValue { get; set; }
    public SearchItemType ItemType { get; set; }
  }
}