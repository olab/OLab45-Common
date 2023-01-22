using Newtonsoft.Json;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLabWebAPI.Dto
{
  public class DynamicScopedObject
  {
    [JsonProperty("counters")]
    public List<CountersDto> Counters { get; set; }

    public DynamicScopedObject()
    {
      Counters = new List<CountersDto>();
    }
  }

  public class ServerDynamicScopedObjects : DynamicScopedObject
  {
    public ServerDynamicScopedObjects()
    {
    }
  }

  public class MapDynamicScopedObjects : DynamicScopedObject
  {
    public MapDynamicScopedObjects()
    {
    }
  }

  public class NodeDynamicScopedObjects : DynamicScopedObject
  {
    public NodeDynamicScopedObjects()
    {
    }
  }

  public class DynamicScopedObjectsDto
  {
    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

    [JsonProperty("checksum")]
    public string Checksum { get; set; }

    [JsonProperty("server")]
    public ServerDynamicScopedObjects Server { get; set; }

    [JsonProperty("map")]
    public MapDynamicScopedObjects Map { get; set; }

    [JsonProperty("node")]
    public NodeDynamicScopedObjects Node { get; set; }

    public DynamicScopedObjectsDto()
    {
      UpdatedAt = DateTime.Now;
      Server = new ServerDynamicScopedObjects();
      Map = new MapDynamicScopedObjects();
      Node = new NodeDynamicScopedObjects();
    }

    public bool IsEmpty()
    {
      return (Map == null) && (Node == null) && (Server == null);
    }

    /// <summary>
    /// Calculate a hash for a given string
    /// </summary>
    /// <param name="key">String to hash</param>
    /// <returns>MD5 hash</returns>
    private string GetStringHash(string plaintext)
    {
      string key = OLabConfiguration.ENCRYPTION_SECRET.Substring(0, 32);
      string cypherString = AesOperation.EncryptString(key, plaintext);
      return cypherString.Substring(cypherString.Length - 6);
    }

    /// <summary>
    /// Generate a verification checksum on the dynamic scope
    /// </summary>
    /// <returns>checksum string</returns>
    public string GenerateChecksum()
    {
      string counterValues = UpdatedAt.ToString();

      foreach (var counter in Server.Counters)
        counterValues += counter.Value;

      foreach (var counter in Node.Counters)
        counterValues += counter.Value;

      foreach (var counter in Map.Counters)
        counterValues += counter.Value;

      return GetStringHash(counterValues);
    }

    /// <summary>
    /// Verify a checksum is valid for a given object
    /// </summary>
    /// <returns>true/false</returns>
    public bool IsValid()
    {
      if (IsEmpty())
        return true;

      return GenerateChecksum() == Checksum;
    }

    /// <summary>
    /// Get counter from scoped object lists
    /// </summary>
    /// <param name="id"></param>
    /// <param name="scopeType">(optional) desired scope level</param>
    /// <returns></returns>
    public CountersDto GetCounter(uint id, string scopeType = "")
    {
      CountersDto dto = null;

      if (string.IsNullOrEmpty(scopeType))
      {
        dto = Server.Counters.FirstOrDefault(x => x.Id == id);
        if (dto == null)
          dto = Map.Counters.FirstOrDefault(x => x.Id == id);
        if (dto == null)
          dto = Node.Counters.FirstOrDefault(x => x.Id == id);

      }

      return dto;
    }
  }
}
