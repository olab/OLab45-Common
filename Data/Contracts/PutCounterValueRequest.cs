using OLab.Api.Dto;

namespace OLab.Data.Contracts;

public class PutCounterValueRequest
{
  public CountersDto Counter { get; set; }
  public DynamicScopedObjectsDto DynamicObjects { get; set; }
}
