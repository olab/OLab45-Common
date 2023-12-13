using Newtonsoft.Json;
using OLab.Api.Utils;
using System;

namespace OLab.Data.Dtos
{
  public class ScopedObjectDto
  {
    [JsonProperty("id")]
    public uint? Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("parentId")]
    public uint ImageableId { get; set; }
    [JsonProperty("scopeLevel")]
    public string ImageableType { get; set; }
    [JsonProperty("createdat")]
    public DateTime? CreatedAt { get; set; }
    [JsonProperty("updatedat")]
    public DateTime? UpdatedAt { get; set; }

    [JsonProperty("scopeLevelObj")]
    public IdName ParentInfo { get; set; }

    public ScopedObjectDto()
    {
      ParentInfo = new IdName();
    }

    public override string ToString()
    {
      return $" '{Name}({Id})'";
    }
  }
}