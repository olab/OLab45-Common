using Newtonsoft.Json;
using System;

namespace OLab.Data.Dtos;

public class CountersDto : ScopedObjectDto
{
  [JsonProperty("value")]
  public string Value { get; set; }

  [JsonProperty("isSystem")]
  public int? IsSystem { get; set; }

  public int ValueAsNumber()
  {
    if (int.TryParse(Value, out var value))
      return value;
    return 0;
  }

  public void SetValue(int value)
  {
    SetValue(value.ToString());
  }

  public void SetValue(string value)
  {
    Value = value;
    UpdatedAt = DateTime.Now;
  }
  public override string ToString()
  {
    return $" '{Name}({Id})' = {Value}";
  }
}