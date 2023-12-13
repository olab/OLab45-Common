using Newtonsoft.Json;
using OLab.Api.Utils;

namespace OLab.Data.Dtos.Designer
{
  public class ScopedObjectDto
  {
    [JsonProperty("id")]
    public uint Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("ScopeLevel")]
    public string ScopeLevel { get; set; }
    [JsonProperty("ScopeId")]
    public uint ParentId { get; set; }
    [JsonProperty("wiki")]
    public string Wiki { get; set; }
    [JsonProperty("acl")]
    public string Acl { get; set; }
    [JsonProperty("isSystem")]
    public int? IsSystem { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("parentInfo")]
    public IdName ParentInfo { get; set; }

    public ScopedObjectDto()
    {
      Acl = "R";
      IsSystem = 0;
    }
  }
}